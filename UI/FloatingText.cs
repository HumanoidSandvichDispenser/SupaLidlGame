using Godot;
using System;

namespace SupaLidlGame.UI
{
    public partial class FloatingText : Node2D
    {
        private Label _label;

        [Export]
        public string Text { get; set; }

        public override void _Ready()
        {
            _label = GetNode<Label>("Label");
            _label.Text = Text;

            Tween tween = GetTree().CreateTween()
                .SetEase(Tween.EaseType.Out)
                .SetTrans(Tween.TransitionType.Quint)
                .SetParallel();

            Random rng = new Random();

            float randomFloat(float min, float max)
            {
                return (float)rng.NextDouble() * (max - min) + min;
            }

            GD.Print(GlobalPosition);
            Position += new Vector2(randomFloat(-8, 8), 0);
            var endPos = Position + new Vector2(0, randomFloat(-16, -8));
            var endMod = new Color(1, 1, 1, 0);
            var endScale = Scale * 0.5f;

            tween.TweenProperty(this, "position", endPos, 0.5f);
            tween.SetTrans(Tween.TransitionType.Linear);
            tween.TweenProperty(this, "modulate", endMod, 0.5f).SetDelay(1.0f);
            tween.TweenProperty(this, "scale", endScale, 0.5f).SetDelay(1.0f);
            tween.TweenCallback(new Callable(this, nameof(OnTweenFinished)))
                .SetDelay(2.5f);

            base._Ready();
        }

        public void OnTweenFinished()
        {
            QueueFree();
        }
    }
}
