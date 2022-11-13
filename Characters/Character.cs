using Godot;

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

        public Vector2 Direction { get; set; } = Vector2.Zero;

        public Vector2 Target { get; set; } = Vector2.Zero;

        [Export]
        public State.Machine StateMachine { get; set; }

        public override void _Process(double delta)
        {
            if (StateMachine != null)
            {
                StateMachine.Process(delta);
            }
        }

        public override void _Input(InputEvent @event)
        {
            if (StateMachine != null)
            {
                StateMachine.Input(@event);
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            if (StateMachine != null)
            {
                StateMachine.PhysicsProcess(delta);
            }
        }
    }
}
