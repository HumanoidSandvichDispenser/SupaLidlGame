using Godot;
using System;

namespace SupaLidlGame.Entities
{
    public partial class Campfire : StaticBody2D
    {
        private PointLight2D _light;

        [Signal]
        public delegate void OnCampfireUse();

        public override void _Ready()
        {
            _light = GetNode<PointLight2D>("PointLight2D");
        }

        public override void _Process(double delta)
        {
            _light.Energy += (GD.Randf() - 0.5f) * 8 * (float)delta;
            _light.Energy = Math.Clamp(_light.Energy, 1.2f, 2.0f);
        }
    }
}
