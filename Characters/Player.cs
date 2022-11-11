using Godot;
using System;

namespace SupaLidlGame.Characters
{
    public partial class Player : Character
    {
        public override void _Ready()
        {
            Speed = 256.0f;
        }

        public override void _Process(double delta)
        {
            Direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        }
    }
}
