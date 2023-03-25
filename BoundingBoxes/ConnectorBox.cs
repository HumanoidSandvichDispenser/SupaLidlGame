using Godot;
using System;
using SupaLidlGame.Characters;

namespace SupaLidlGame.BoundingBoxes
{
    public partial class ConnectorBox : Area2D
    {
        [Signal]
        public delegate void RequestedEnterEventHandler(
            ConnectorBox box,
            Player player);

        [Export]
        public string ToArea { get; set; }

        [Export]
        public string ToConnector { get; set; }

        /// <summary>
        /// Determines if the connector requires the user to interact to enter
        /// the connector
        /// </summary>
        [Export]
        public bool RequiresInteraction { get; set; } = true;

        [Export]
        public CollisionShape2D Collision { get; set; }

        private Player _player = null;

        public override void _Ready()
        {
            if (Collision is null)
            {
                throw new NullReferenceException("Collision not specified");
            }

            BodyEntered += (Node2D body) =>
            {
                if (body is Player player)
                {
                    _player = player;
                }
            };

            BodyExited += (Node2D body) =>
            {
                if (body is Player)
                {
                    _player = null;
                }
            };
        }

        public override void _Process(double delta)
        {
            if (Input.IsActionJustPressed("interact"))
            {
                EmitSignal(SignalName.RequestedEnter, this, _player);
            }

            base._Process(delta);
        }
    }
}
