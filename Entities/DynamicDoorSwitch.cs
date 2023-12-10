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
    public Godot.Collections.Array<NodePath> VisibleOnToggle { get; set; }

    private AnimationPlayer _animPlayer;

    public override void _Ready()
    {
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        var globalState = this.GetGlobalState();
        var doorState = globalState.MapState[MapStateKey];
        InteractionTrigger.Interaction += OnInteraction;

        // disable specific animations when entering initial state
        SetAnimations(false);
        RefreshMapState(doorState);
    }

    private void RefreshMapState(Variant value)
    {
        if (value.Equals(default) || value.VariantType != Variant.Type.Bool)
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
        Variant oldState = globalState.MapState[MapStateKey];
        bool newState;
        globalState.MapState[MapStateKey] = newState = oldState.VariantType switch
        {
            Variant.Type.Bool => !(bool)oldState,
            _ => true,
        };
        SetAnimations(true);
        RefreshMapState(newState);

        GD.Print($"Map state \"{MapStateKey}\" {oldState} -> {newState}");
    }

    public void SetAnimations(bool isEnabled)
    {
        foreach (var animKey in _animPlayer.GetAnimationList())
        {
            var anim = _animPlayer.GetAnimation(animKey);
            for (int i = 0; i < anim.GetTrackCount(); i++)
            {
                foreach (var nodePath in VisibleOnToggle)
                {
                    if (anim.TrackGetPath(i) == nodePath)
                    {
                        GD.Print($"Disabled anim for {nodePath}");
                        anim.TrackSetEnabled(i, isEnabled);
                    }
                }
            }
        }
    }
}
