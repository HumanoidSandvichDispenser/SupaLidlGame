using Godot;
using SupaLidlGame.Audio;
using SupaLidlGame.Characters;
using SupaLidlGame.Extensions;
using SupaLidlGame.Scenes;
using SupaLidlGame.State.Global;
using SupaLidlGame.Events;

namespace SupaLidlGame.Utils;

public partial class World : Node
{
    public static World Instance { get; private set; }

    [Export]
    public Map CurrentMap { get; protected set; }

    [Export]
    public Player CurrentPlayer { get; set; }

    [Export]
    public Boss CurrentBoss { get; set; }

    public UI.UIController UIController { get; set; }

    [Export]
    public AudioStreamPlayer MusicPlayer { get; set; }

    [Export]
    public Dialogue.Balloon DialogueBalloon
    {
        get
        {
            if (!IsDialogueOpen)
            {
                var scene = GD.Load<PackedScene>("res://Dialogue/balloon.tscn");
                _dialogueBalloon = scene.Instantiate<Dialogue.Balloon>();
                _uiViewport.AddChild(_dialogueBalloon);
            }
            return _dialogueBalloon;
        }
        set
        {
            if (_dialogueBalloon != value && _dialogueBalloon is not null)
            {
                _dialogueBalloon.QueueFree();
            }
            _dialogueBalloon = value;
        }
    }

    public bool IsDialogueOpen
    {
        get => _dialogueBalloon is not null && IsInstanceValid(_dialogueBalloon);
    }

    private Dialogue.Balloon _dialogueBalloon;

    private SubViewport _uiViewport;

    public State.Global.GlobalState GlobalState { get; set; }

    public EventBus EventBus { get; set; }

    private CacheStore<string, Map> _maps = new();

    private string _currentConnector;

    private string _currentMapResourcePath;

    internal DebugCommands Debug { get; }

    //private Entities.Campfire _lastCampfire = null;

    private const string PLAYER_PATH = "res://Characters/Player.tscn";
    private PackedScene _playerScene;

    public World()
    {
        _playerScene = ResourceLoader.Load<PackedScene>(PLAYER_PATH);
        if (Instance is null)
        {
            Instance = this;
        }
        else
        {
            throw new System.Exception("Another World instance is running.");
        }

        Debug = new(this);
    }

    ~World()
    {
        Debug.Free();
    }

    public override void _Ready()
    {
        // check if world already exists

        GlobalState = this.GetGlobalState();

        Godot.RenderingServer.SetDefaultClearColor(Godot.Colors.Black);

        UIController = this.GetMainUI();

        EventBus = this.GetEventBus();
        EventBus.RequestMoveToArea += (RequestAreaArgs args) =>
        {
            //CallDeferred(nameof(MoveToArea), args.Area, args.Connector);
            MoveToArea(args.Area, args.Connector);
        };
        EventBus.RegisteredBoss += RegisterBoss;
        EventBus.DeregisteredBoss += DeregisterBoss;

        _uiViewport = GetNode<SubViewport>("CanvasLayer/SubViewportContainer/UIViewport");

        // create a player (currently unparented)
        CreatePlayer();

        // TODO: create start menu and load game from there
        LoadGame();

        base._Ready();
    }

    private void RegisterBoss(Boss boss)
    {
        if (boss is null)
        {
            DeregisterBoss(CurrentBoss);
            return;
        }
        CurrentBoss = boss;
        GetNode<AudioManager>("/root/AudioManager").PlayBackground(boss.Music, 2);
    }

    private void DeregisterBoss(Boss boss)
    {
        CurrentBoss = null;
        GetNode<AudioManager>("/root/AudioManager").StopBackground(2);
    }

    private void LoadMap(Map map)
    {
        GD.Print("Loading map " + map.Name);

        var root = GetTree().Root;

        if (CurrentMap is not null)
        {
            _maps.Update(CurrentMap.SceneFilePath);
            CurrentMap.Entities.RemoveChild(CurrentPlayer);
            root.RemoveChild(CurrentMap);
            CurrentMap.Active = false;
        }

        root.AddChild(map);

        CurrentMap = map;
        CurrentMap.Active = true;
        CurrentMap.Load();

        EventBus.EmitSignal(EventBus.SignalName.AreaChanged, map);

        if (CurrentPlayer is not null)
        {
            CurrentMap.Entities.AddChild(CurrentPlayer);
        }
    }

    public void LoadScene(Map map)
    {
        _maps.Update(map.SceneFilePath, map);
        LoadMap(map);
    }

    public void LoadScene(PackedScene scene)
    {
        Map map;
        string path = scene.ResourcePath;

        if (_maps.IsItemValid(path))
        {
            GD.Print($"{path} is cached");
            map = _maps.Retrieve(path);
        }
        else
        {
            if (_maps.IsItemStale(path))
            {
                _maps[path].Value.QueueFree();
                GD.Print("Freeing stale map " + path);
            }
            map = scene.Instantiate<Map>();
            _maps.Update(path, map);
        }

        LoadMap(map);
    }

    public void LoadScene(string path)
    {
        Map map;
        if (_maps.IsItemValid(path))
        {
            GD.Print($"{path} is cached");
            map = _maps.Retrieve(path);
        }
        else
        {
            if (_maps.IsItemStale(path))
            {
                _maps[path].Value.QueueFree();
                GD.Print("Freeing stale map " + path);
            }
            var scene = ResourceLoader.Load<PackedScene>(path);
            map = scene.Instantiate<Map>();
            _maps.Update(scene.ResourcePath, map);
        }

        LoadMap(map);
    }

    public Player CreatePlayer()
    {
        CurrentPlayer = _playerScene.Instantiate<Player>();

        CurrentPlayer.Death += (HurtArgs args) =>
        {
            // TODO: respawn the player at the last campfire.
            GetTree().CreateTimer(3).Timeout += () =>
            {
                SpawnPlayer();
            };
            GlobalState.Stats.DeathCount++;

            //EventBus.EmitSignal(EventBus.SignalName.DeregisteredBoss,
            //    CurrentBoss);
        };

        return CurrentPlayer;
    }

    private void MovePlayerToConnector(string name)
    {
        var marker = CurrentMap.Markers.GetNode<Marker2D>(name);
        CurrentPlayer.GlobalPosition = marker?.GlobalPosition ?? Vector2.Zero;
    }

    public async void MoveToArea(string path, string connector)
    {
        _currentConnector = connector;
        GD.Print($"Moving to area {path} - {connector}");

        EventBus.EmitSignal(EventBus.SignalName.EnterTransition);
        GetTree().Paused = true;
        await ToSignal(EventBus, EventBus.SignalName.TransitionFinished);

        if (path != CurrentMap.SceneFilePath)
        {
            LoadScene(path);
        }

        GetTree().Paused = false;
        EventBus.EmitSignal(EventBus.SignalName.ExitTransition);

        // after finished loading, move our player to the connector
        MovePlayerToConnector(connector);
    }

    public void SaveGame()
    {
        SetSpawn(CurrentPlayer.GlobalPosition);

        var save = GetSave();
        GlobalState.ExportToSave(save);
        ResourceSaver.Save(save, "user://save.tres");
    }

    private Save GetSave()
    {
        if (ResourceLoader.Exists("user://save.tres"))
        {
            return ResourceLoader.Load<Save>("user://save.tres");
        }
        else
        {
            return ResourceLoader.Load("res://Assets/default-save.tres")
                .Duplicate(true) as Save;
        }
    }

    public void LoadGame()
    {
        GlobalState.ImportFromSave(GetSave());

        // load the player scene
        // TODO: implement
    }

    /// <summary>
    /// Sets the player's saved spawn position.
    /// </summary>
    /// <param name="position">The position to save and spawn the player in</param>
    /// <param name="mapKey">
    /// The map to spawn the player in. If <see langword="null" />, use the
    /// <c>World</c>'s <c>CurrentMap</c>
    /// </param>
    public void SetSpawn(Vector2 position, string mapKey = null)
    {
        GD.Print("Set spawn");
        if (mapKey is null)
        {
            mapKey = CurrentMap.SceneFilePath;
        }
        GlobalState.Stats.SaveLocation = position;
        GlobalState.Stats.SaveMapKey = mapKey;
    }

    public void SpawnPlayer()
    {
        // TODO: add max health property
        if (CurrentMap.SceneFilePath != GlobalState.Stats.SaveMapKey)
        {
            LoadScene(GlobalState.Stats.SaveMapKey);
        }
        CurrentPlayer.GlobalPosition = GlobalState.Stats.SaveLocation;
        CurrentPlayer.Spawn();
    }

    public void StaleCache()
    {
        foreach (var kv in _maps)
        {
            var map = kv.Value?.Value;
            if (map is not null && IsInstanceValid(map))
            {
                GD.Print($"Staling {kv.Key}");
                kv.Value.Stale();
            }
        }
    }

    public Node FindEntity(string name) => CurrentMap.Entities.GetNode(name);
}

internal partial class DebugCommands : Godot.GodotObject
{
    private World _world;

    internal DebugCommands(World world)
    {
        _world = world;
    }

    internal Items.ItemMetadata GiveItem(string item)
    {
        var itemMetadata = ResourceLoader
            .Load<Items.ItemMetadata>($"res://Items/{item}.tres");
        _world.CurrentPlayer.Inventory.Add(itemMetadata);
        return itemMetadata;
    }
}
