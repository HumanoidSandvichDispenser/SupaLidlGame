using Godot;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.Entities;

public partial class DynamicDoor : StaticBody2D
{
    [Export]
    public string MapStateKey { get; set; }

    [Export]
    public Godot.Collections.Array<NodePath> VisibleOnToggle { get; set; } = new();

    [Export]
    public bool DefaultState { get; set; }

    private AnimationPlayer _animPlayer;

    public override void _Ready()
    {
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        var globalState = this.GetGlobalState();
        globalState.MapState.MapStateBoolChanged += OnMapStateChanged;
        SetAnimations(false);
        RefreshMapState((bool)globalState.MapState[MapStateKey]);
    }

    public virtual void OnMapStateChanged(string key, bool value)
    {
        GD.Print("Map state changed");
        if (key == MapStateKey)
        {
            SetAnimations(true);
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
        //this.GetWorld().CurrentPlayer.Camera.Shake(1, 0.5f);
    }

    public virtual void Close()
    {
        _animPlayer?.TryPlay("close");
        //this.GetWorld().CurrentPlayer.Camera.Shake(2, 0.25f);
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
                        anim.TrackSetEnabled(i, isEnabled);
                    }
                }
            }
        }
    }
}
