using Godot;
using SupaLidlGame.Extensions;
using System;

namespace SupaLidlGame.Prototyping
{
    public partial class ContextBasedSteering : Node2D
    {
        public float PreferredDistance { get; set; } = 256.0f;
        Vector2 _direction;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            GD.Print("Started ContextBasedSteering test");
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
            _direction = GetDirection();
            QueueRedraw();
        }

        public Vector2 GetDirection()
        {
            float directWeight;
            float strafeWeight;

            Vector2 towards = GetGlobalMousePosition() - GlobalPosition;
            float dist = towards.Length();

            Vector2 directDir = towards.Normalized();
            Vector2 strafeDir = directDir.Clockwise90();

            // weights approach 1
            // dy/dx = 1 - y
            // y = 1 - e^(-x)

            directWeight = 1 - Mathf.Pow(Mathf.E, -(dist / PreferredDistance));
            strafeWeight = 1 - directWeight;

            /*
            Vector2 midpoint = (strafeDir * strafeWeight)
                .Midpoint(directDir * directWeight);
            */
            Vector2 midpoint = (directDir * directWeight)
                .Midpoint(strafeDir * strafeWeight);

            return midpoint.Normalized();
        }

        public override void _Draw()
        {
            DrawLine(Vector2.Zero, _direction * 256, Colors.Green);
        }
    }
}
