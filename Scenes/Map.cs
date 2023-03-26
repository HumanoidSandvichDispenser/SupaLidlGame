using Godot;
using System;

namespace SupaLidlGame.Scenes
{
    public partial class Map : TileMap
    {
        [Export]
        public Node2D Entities { get; set; }

        [Export]
        public Node2D Areas { get; set; }

        [Export]
        public Node2D Spawners { get; set; }

        [Export]
        public Vector2 CameraLowerBound { get; set; }

        [Export]
        public Vector2 CameraUpperBound { get; set; }

        private bool _active;

        public bool Active
        {
            get
            {
                return _active;
            }
            set
            {
                _active = Visible =  value;
                SetProcess(value);
            }
        }

        public override void _Ready()
        {
            Active = true;
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
        }
    }
}
