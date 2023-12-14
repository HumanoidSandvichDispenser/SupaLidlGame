using Godot;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.Entities;

public partial class DynamicDoor : StaticBody2D
{
    [Export]
    public string MapStateKey { get; set; }

    [Export]
    public Godot.Collections.Array<NodePath> VisibleOnToggle { get; set; }

    [Export]
    public Godot.Collections.Array<NavigationRegion2D> Rebake { get; set; }

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

    private async void RefreshMapState(bool value)
    {
        if (value)
        {
            Open();
        }
        else
        {
            Close();
        }

        await ToSignal(_animPlayer,
            AnimationPlayer.SignalName.AnimationFinished);

        if (Rebake is not null)
        {
            foreach (var navmesh in Rebake)
            {
                // rebake navmesh so NPCs can correctly travel conditionally
                GD.Print("rebaking");
                navmesh.BakeNavigationPolygon();
            }
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
        if (VisibleOnToggle is null)
        {
            GD.PushWarning($"DynamicDoor {GetPath()} has no VisibleOnToggle " +
                "array. Set this to empty array to disable this warning.");
            return;
        }

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
