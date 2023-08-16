using Godot;
using System;

namespace SupaLidlGame.UI;

public partial class FloatingText : Node2D
{
    private string _text;

    [Export]
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            if (_label is not null)
            {
                _label.Text = value;
            }
        }
    }

    public Timer Timer { get; set; }

    protected Label _label;

    protected Tween _tween;

    public override void _Ready()
    {
        _label = GetNode<Label>("Label");
        _label.Text = _text;

        Timer = GetNode<Timer>("Timer");
        Timer.Timeout += Die;

        ShowText();
    }

    public void Reset()
    {
        Modulate = Colors.White;
        Scale = Vector2.One * 0.5f;
    }

    public void ShowText()
    {
        Reset();

        if (_tween is not null && _tween.IsRunning())
        {
            _tween.Kill();
        }

        _tween = GetTree().CreateTween()
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Quint)
            .SetParallel();

        Random rng = new Random();

        float randomFloat(float min, float max)
        {
            return (float)rng.NextDouble() * (max - min) + min;
        }

        Position += new Vector2(randomFloat(-8, 8), 0);
        var endPos = Position + new Vector2(0, randomFloat(-16, -8));
        var endMod = new Color(1, 1, 1, 0);
        var endScale = Scale * 0.5f;

        _tween.TweenProperty(this, "position", endPos, 0.5f);
        _tween.SetTrans(Tween.TransitionType.Linear);
        _tween.TweenProperty(this, "modulate", endMod, 0.5f).SetDelay(1.0f);
        _tween.TweenProperty(this, "scale", endScale, 0.5f).SetDelay(1.0f);
        _tween.Play();
    }

    public void Die()
    {
        QueueFree();
    }
}
