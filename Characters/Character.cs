using Godot;

namespace SupaLidlGame.Characters
{
    public partial class Character : CharacterBody2D
    {
        [Export]
        public float Speed { get; protected set; } = 32.0f;

        [Export]
        public float Mass
        {
            get => _mass;
            set
            {
                if (value > 0)
                    _mass = value;
            }
        }

        protected float _mass = 1.0f;

        public float JumpVelocity { get; protected set; } = -400.0f;

        public float AccelerationMagnitude { get; protected set; } = 256.0f;

        public Vector2 Acceleration => Direction * AccelerationMagnitude;

        public Vector2 Direction { get; set; } = Vector2.Zero;

        public Vector2 Target { get; set; } = Vector2.Zero;

        public float Health { get; set; }

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

        public void ApplyImpulse(Vector2 impulse, bool resetVelocity = false)
        {
            // delta p = F delta t
            if (resetVelocity)
                Velocity = Vector2.Zero;
            Velocity += impulse / Mass;
        }

        public void _on_hurtbox_received_damage(float damage)
        {
            Health -= damage;
        }
    }
}
