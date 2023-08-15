using Godot;
using SupaLidlGame.Characters;
using SupaLidlGame.Extensions;
using SupaLidlGame.Scenes;
using SupaLidlGame.State.Global;

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

    public Events.EventBus EventBus { get; set; }

    private CacheStore<string, Map> _maps = new();

    private string _currentConnector;

    private string _currentMapResourcePath;

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
    }

    public override void _Ready()
    {
        // check if world already exists

        GlobalState = this.GetGlobalState();

        Godot.RenderingServer.SetDefaultClearColor(Godot.Colors.Black);

        UIController = this.GetMainUI();

        EventBus = this.GetEventBus();
        EventBus.RequestMoveToArea += (Events.RequestAreaArgs args) =>
        {
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
        CurrentBoss = boss;
        MusicPlayer.Stream = boss?.Music;
        // TODO: use an audio manager
        if (MusicPlayer.Stream is null)
        {
            MusicPlayer.Stop();
        }
        else
        {
            MusicPlayer.Play();
        }
    }

    private void DeregisterBoss(Boss boss)
    {
        CurrentBoss = null;
        MusicPlayer.Stop();
    }

    private void LoadMap(Map map)
    {
        GD.Print("Loading map " + map.Name);

        if (CurrentMap is not null)
        {
            CurrentMap.Entities.RemoveChild(CurrentPlayer);
            GetTree().Root.RemoveChild(CurrentMap);
            CurrentMap.Active = false;
        }

        GetTree().Root.AddChild(map);
        InitTilemap(map);

        CurrentMap = map;
        CurrentMap.Active = true;
        CurrentMap.Load();

        EventBus.EmitSignal(Events.EventBus.SignalName.AreaChanged, map);

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
        if (CurrentMap is not null)
        {
            _maps.Update(CurrentMap.SceneFilePath);
        }

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

        CurrentPlayer.Death += (Events.HurtArgs args) =>
        {
            // TODO: respawn the player at the last campfire.
            GetTree().CreateTimer(3).Timeout += () =>
            {
                SpawnPlayer();
            };
        };

        /*
        CurrentPlayer.Hurt += (Events.HurtArgs args) =>
        {
            // TODO: move this to UI controller and add a setup method
            var bar = UIController.GetNode<UI.HealthBar>("Top/Margin/HealthBar");
            bar.ProgressBar.Value = args.NewHealth;
        };
        */

        return CurrentPlayer;
    }

    private void InitTilemap(Map map)
    {
        // this is being replaced with interaction triggers
    }

    private void MovePlayerToConnector(string name)
    {
        // find the first connector with the specified name
        // TODO: replace this with event buses
        //var connector = CurrentMap.Areas.GetChildren().First((child) =>
        //{
        //    if (child is BoundingBoxes.ConnectorBox connector)
        //    {
        //        return connector.Identifier == name;
        //    }
        //    return false;
        //}) as BoundingBoxes.ConnectorBox;

        //CurrentPlayer.GlobalPosition = connector.GlobalPosition;
        CurrentPlayer.GlobalPosition = Vector2.Zero;
    }

    public void MoveToArea(string path, string connector)
    {
        _currentConnector = connector;
        GD.Print($"Moving to area {path} - {connector}");
        if (path != CurrentMap.SceneFilePath)
        {
            LoadScene(path);
            //_currentMapResourcePath = path;
        }

        // after finished loading, move our player to the connector
        MovePlayerToConnector(connector);
    }

    public void _on_area_2d_requested_enter(
        BoundingBoxes.ConnectorBox box,
        Player player)
    {
        GD.Print("Requesting to enter " + box.ToConnector);
        MoveToArea(box.ToArea, box.ToConnector);
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
        //CurrentPlayer.Health = 100;
        //CurrentPlayer.Sprite.Visible = true;
        if (CurrentMap.SceneFilePath != GlobalState.Stats.SaveMapKey)
        {
            LoadScene(GlobalState.Stats.SaveMapKey);
        }
        CurrentPlayer.GlobalPosition = GlobalState.Stats.SaveLocation;
        CurrentPlayer.Spawn();
    }

    public Node FindEntity(string name) => CurrentMap.Entities.GetNode(name);
}
