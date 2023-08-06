using Godot;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.Entities;

public partial class DynamicDoor : StaticBody2D
{
    [Export]
    public string MapStateKey { get; set; }

    [Export]
    public Godot.Collections.Array<Node2D> VisibleOnToggle { get; set; } = new();

    [Export]
    public bool DefaultState { get; set; }

    private AnimationPlayer _animPlayer;

    public override void _Ready()
    {
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        var globalState = this.GetGlobalState();
        globalState.MapState.MapStateBoolChanged += OnMapStateChanged;
        RefreshMapState((bool)globalState.MapState[MapStateKey]);
    }

    public virtual void OnMapStateChanged(string key, bool value)
    {
        GD.Print("Map state changed");
        if (key == MapStateKey)
        {
            foreach (Node2D node in VisibleOnToggle)
            {
                // this is so extra effects are not played or showed when the
                // door opens/closes from loading the map state.
                node.Visible = true;
            }
            RefreshMapState(value);
        }
    }

    private void RefreshMapState(bool value)
    {
        if (value)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    public virtual void Open()
    {
        _animPlayer?.TryPlay("open");
    }

    public virtual void Close()
    {
        _animPlayer?.TryPlay("cose");
    }
}
