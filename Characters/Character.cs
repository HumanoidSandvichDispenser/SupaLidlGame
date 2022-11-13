using Godot;
using System;

namespace SupaLidlGame.Characters
{
    public partial class Character : CharacterBody2D
    {
        [Export]
        public float Speed { get; protected set; } = 128.0f;

        public float JumpVelocity { get; protected set; } = -400.0f;
        public float AccelerationMagnitude { get; protected set; } = 256.0f;

        public Vector2 Acceleration => Direction * AccelerationMagnitude;

        // Get the gravity from the project settings to be synced with RigidBody nodes.
        public float Gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

        public Vector2 Direction { get; protected set; } = Vector2.Zero;

        public override void _Process(double delta)
        {
        }

        public override void _PhysicsProcess(double delta)
        {
            // movement would be more crisp with no acceleration
            Velocity = Direction * Speed;
            MoveAndSlide();
        }
    }
}
