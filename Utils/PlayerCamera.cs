using Godot;
using System;

namespace SupaLidlGame.Utils
{
    public partial class PlayerCamera : Camera2D
    {
        protected float _intensity;

        protected double _timeLeft;

        public override void _Ready()
        {

        }

        public override void _Process(double delta)
        {
            if (_timeLeft > 0)
            {
                _timeLeft -= delta;
                Offset = RandomOffset(_intensity);
            }
            else
            {
                Offset = Vector2.Zero;
            }

            if (_intensity > 0)
            {
                _intensity = Mathf.MoveToward(_intensity, 0.0f, (float)delta);
            }
        }

        public void Shake(float intensity, double time)
        {
            _intensity = Mathf.Max(_intensity, intensity);
            _timeLeft = Math.Max(_timeLeft, time);
        }

        private Vector2 RandomOffset(float intensity)
        {
            Vector2 ret = Vector2.Zero;
            var rng = new RandomNumberGenerator();
            ret.x = (rng.Randf() - 0.5f) * intensity;
            ret.y = (rng.Randf() - 0.5f) * intensity;
            return ret;
        }
    }
}
