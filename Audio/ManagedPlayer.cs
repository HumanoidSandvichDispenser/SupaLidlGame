using Godot;

namespace SupaLidlGame.Audio;

public sealed partial class ManagedAudioPlayer : AudioStreamPlayer
{
    private Tween _tween;

    public bool IsDead { get; set; }

    private Tween GetNewTween()
    {
        if (_tween is not null && IsInstanceValid(_tween))
        {
            _tween.Kill();
        }

        return _tween = GetTree().CreateTween().BindNode(this);
    }

    public void FadeOut(float time, bool kill = false, bool pause = false)
    {
        GetNewTween();
        _tween.TweenProperty(this, "volume_db", -80, time);
        if (kill)
        {
            IsDead = true;
            _tween.TweenCallback(Callable.From(QueueFree));
        }
        else if (pause)
        {
            _tween.TweenCallback(Callable.From(() => StreamPaused = true));
        }
    }

    public void FadeIn(float time, bool unpause = false)
    {
        GetNewTween();
        _tween.TweenProperty(this, "volume_db", 0, time);
        if (unpause)
        {
            _tween.TweenCallback(Callable.From(() => StreamPaused = false));
        }
    }
}
