using Godot;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.Entities;

public partial class DynamicDoorSwitch : StaticBody2D
{
    [Export]
    public BoundingBoxes.InteractionTrigger InteractionTrigger { get; set; }

    [Export]
    public string MapStateKey { get; set; }

    [Export]
    public Godot.Collections.Array<Node2D> VisibleOnToggle { get; set; } = new();

    private AnimationPlayer _animPlayer;

    public override void _Ready()
    {
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        var globalState = this.GetGlobalState();
        var doorState = globalState.MapState[MapStateKey];
        if (!doorState.Equals(default))
        {
            if (!(bool)doorState)
            {
                InteractionTrigger.Interaction += OnInteraction;
            }
        }
    }

    private void RefreshMapState(Variant value)
    {
        if (value.Equals(default))
        {
            return;
        }

        if ((bool)value)
        {
            _animPlayer?.TryPlay("on");
        }
        else
        {
            _animPlayer?.TryPlay("off");
        }
    }

    public void OnInteraction()
    {
        var globalState = this.GetGlobalState();
        globalState.MapState[MapStateKey] = true;
        RefreshMapState(true);

        GD.Print($"{MapStateKey} is now on");

        foreach (Node2D node in VisibleOnToggle)
        {
            node.Visible = true;
        }
    }
}
