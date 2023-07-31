using Godot;
using SupaLidlGame.Characters;
using SupaLidlGame.Extensions;
using SupaLidlGame.Scenes;
using System.Collections.Generic;
using System.Linq;

namespace SupaLidlGame.Utils;

public partial class World : Node2D
{
    [Export]
    public PackedScene StartingArea { get; set; }

    [Export]
    public Map CurrentMap { get; protected set; }

    [Export]
    public Player CurrentPlayer { get; set; }

    [Export]
    public Boss CurrentBoss { get; set; }

    [Export]
    public UI.UIController UIController { get; set; }

    [Export]
    public AudioStreamPlayer MusicPlayer { get; set; }

    [Export]
    public Dialogue.Balloon DialogueBalloon { get; set; }

    private Dictionary<string, Map> _maps;

    private string _currentConnector;

    private string _currentMapResourcePath;

    //private Entities.Campfire _lastCampfire = null;
    public Vector2 SaveLocation { get; set; }
    public string SaveMapKey { get; set; }

    private const string PLAYER_PATH = "res://Characters/Player.tscn";
    private PackedScene _playerScene;

    public World()
    {
        _maps = new Dictionary<string, Map>();
        _playerScene = ResourceLoader.Load<PackedScene>(PLAYER_PATH);
    }

    public override void _Ready()
    {
        // check if world already exists

        var globalState = GetNode<State.Global.GlobalState>("/root/GlobalState");
        if (globalState.World is not null)
        {
            throw new System.InvalidOperationException();
        }

        globalState.World = this;

        Godot.RenderingServer.SetDefaultClearColor(Godot.Colors.Black);

        if (StartingArea is not null)
        {
            LoadScene(StartingArea);
        }

        // spawn the player in
        CreatePlayer();

        CurrentPlayer.Death += (Events.HealthChangedArgs args) =>
        {
            // TODO: respawn the player at the last campfire.
            GetTree().CreateTimer(1).Timeout += () =>
            {
                SpawnPlayer();
            };
        };

        CurrentPlayer.Hurt += (Events.HealthChangedArgs args) =>
        {
            // TODO: move this to UI controller and add a setup method
            var bar = UIController.GetNode<UI.HealthBar>("Top/Margin/HealthBar");
            bar.ProgressBar.Value = args.NewHealth;
        };

        base._Ready();
    }

    public void RegisterBoss(Boss boss)
    {
        CurrentBoss = boss;
        UIController.BossBar.Boss = boss;
        MusicPlayer.Stream = boss.Music;
        MusicPlayer.Play();
    }

    public void DeregisterBoss(Boss boss)
    {
        CurrentBoss = null;
        UIController.BossBar.Boss = null;
        MusicPlayer.Stop();
    }

    private void LoadMap(Map map)
    {
        GD.Print("Loading map " + map.Name);

        if (CurrentMap is not null)
        {
            CurrentMap.Entities.RemoveChild(CurrentPlayer);
            RemoveChild(CurrentMap);
            CurrentMap.Active = false;
        }

        AddChild(map);
        InitTilemap(map);

        CurrentMap = map;
        CurrentMap.Active = true;
        CurrentMap.Load();

        if (CurrentPlayer is not null)
        {
            CurrentMap.Entities.AddChild(CurrentPlayer);
        }
    }

    public void LoadScene(PackedScene scene)
    {
        Map map;
        if (_maps.ContainsKey(scene.ResourcePath))
        {
            map = _maps[scene.ResourcePath];
        }
        else
        {
            map = scene.Instantiate<Map>();
            _maps.Add(scene.ResourcePath, map);
        }

        LoadMap(map);
    }

    public void LoadScene(string path)
    {
        Map map;
        if (_maps.ContainsKey(path))
        {
            map = _maps[path];
        }
        else
        {
            var scene = ResourceLoader.Load<PackedScene>(path);
            map = scene.Instantiate<Map>();
            _maps.Add(scene.ResourcePath, map);
        }

        LoadMap(map);
    }

    public void CreatePlayer()
    {
        CurrentPlayer = _playerScene.Instantiate<Player>();
        CurrentMap.Entities.AddChild(CurrentPlayer);
    }

    private void InitTilemap(Map map)
    {
        var children = map.Areas.GetChildren();
        foreach (Node node in children)
        {
            if (node is BoundingBoxes.ConnectorBox connector)
            {
                // this reconnects the EventHandler if it is connected
                connector.RequestedEnter -= _on_area_2d_requested_enter;
                connector.RequestedEnter += _on_area_2d_requested_enter;
            }
        }
    }

    private void MovePlayerToConnector(string name)
    {
        // find the first connector with the specified name
        var connector = CurrentMap.Areas.GetChildren().First((child) =>
        {
            if (child is BoundingBoxes.ConnectorBox connector)
            {
                return connector.Identifier == name;
            }
            return false;
        }) as BoundingBoxes.ConnectorBox;

        CurrentPlayer.GlobalPosition = connector.GlobalPosition;
    }

    public void MoveToArea(string path, string connector)
    {
        _currentConnector = connector;
        if (path != _currentMapResourcePath)
        {
            var scene = ResourceLoader.Load<PackedScene>(path);
            LoadScene(scene);
            _currentMapResourcePath = path;
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
        throw new System.NotImplementedException();
    }

    public void LoadGame()
    {
        throw new System.NotImplementedException();
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
            SaveLocation = position;
            SaveMapKey = mapKey;
        }
    }

    public void SpawnPlayer()
    {
        // TODO: add max health property
        //CurrentPlayer.Health = 100;
        //CurrentPlayer.Sprite.Visible = true;
        if (CurrentMap.SceneFilePath != SaveMapKey)
        {
            LoadScene(SaveMapKey);
        }
        CurrentPlayer.GlobalPosition = SaveLocation;
        CurrentPlayer.Spawn();
    }

    public Node FindEntity(string name) => CurrentMap.Entities.GetNode(name);
}
