using Godot;
using SupaLidlGame.Extensions;
using System.Collections.Generic;

namespace SupaLidlGame.Scenes;

public partial class Map : TileMap
{
    [Export]
    public Node2D Entities { get; set; }

    [Export]
    public Node2D Areas { get; set; }

    [Export]
    public Node2D Spawners { get; set; }

    [Export]
    public Node2D Markers { get; set; }

    [Export]
    public Vector2 CameraLowerBound { get; set; }

    [Export]
    public Vector2 CameraUpperBound { get; set; }

    [Export]
    public Color ClearColor { get; set; }

    [Export]
    public string AreaName { get; set; }

    [Export]
    public string MapName { get; set; }

    [Export]
    public AudioStream Music { get; set; }

    private bool _active;

    public bool Active
    {
        get
        {
            return _active;
        }
        set
        {
            _active = value;
            SetProcess(value);
        }
    }

    public override void _Ready()
    {
        var world = this.GetWorld();
        if (world.CurrentMap is null)
        {
            world.LoadScene(this);
        }
        Active = true;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public void Load()
    {
        Godot.RenderingServer.SetDefaultClearColor(ClearColor);
        GetNode<Audio.AudioManager>("/root/AudioManager").PlayBackground(Music);
    }

    public IEnumerable<Characters.Character> GetCharacters()
    {
        foreach (var child in Entities.GetChildren())
        {
            if (child is Characters.Character c)
            {
                yield return c;
            }
        }
    }

    public Node SpawnEntity(PackedScene scene)
    {
        var instance = scene.Instantiate();
        Entities.AddChild(instance);
        return instance;
    }

    public T SpawnEntity<T>(PackedScene scene) where T : Node
    {
        return SpawnEntity(scene) as T;
    }
}
