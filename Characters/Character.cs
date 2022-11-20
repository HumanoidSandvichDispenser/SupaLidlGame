using Godot;
using SupaLidlGame.Items;
using SupaLidlGame.Utils;

namespace SupaLidlGame.Characters
{
    public partial class Character : CharacterBody2D, IFaction
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

        public float Health
        {
            get => _health;
            set
            {
                if (!IsAlive && value < 0)
                {
                    return;
                }

                _health = value;
                GD.Print(_health);
                if (_health <= 0)
                {
                    Die();
                }
            }
        }

        protected float _health = 100f;

        public bool IsAlive => Health > 0;

        [Export]
        public AnimatedSprite2D Sprite { get; set; }

        [Export]
        public Inventory Inventory { get; set; }

        [Export]
        public State.Machine StateMachine { get; set; }

        [Export]
        public ushort Faction { get; set; }

        public override void _Process(double delta)
        {
            if (StateMachine != null)
            {
                StateMachine.Process(delta);
            }

            Sprite.FlipH = Target.x < 0;
            DrawTarget();
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

        /// <summary>
        /// Modify the <c>Character</c>'s velocity
        /// </summary>
        public virtual void ModifyVelocity()
        {

        }

        public virtual void Die()
        {
            GD.Print("lol died");
            QueueFree();
        }

        public void ApplyImpulse(Vector2 impulse, bool resetVelocity = false)
        {
            // delta p = F delta t
            if (resetVelocity)
                Velocity = Vector2.Zero;
            Velocity += impulse / Mass;
        }

        protected void DrawTarget()
        {
            Vector2 target = Target;
            float angle = Mathf.Atan2(target.y, Mathf.Abs(target.x));
            Vector2 scale = Inventory.Scale;
            if (target.x < 0)
            {
                scale.y = -1;
                angle = Mathf.Pi - angle;
            }
            else
            {
                scale.y = 1;
            }
            Inventory.Scale = scale;
            Inventory.Rotation = angle;
        }

        public void _on_hurtbox_received_damage(float damage,
            Character inflictor,
            float knockback,
            Vector2 knockbackOrigin = default,
            Vector2 knockbackVector = default)
        {
            Health -= damage;
            /*
            Vector2 knockbackDir = knockbackVector;
            if (knockbackDir == default)
            {
                if (knockbackOrigin == default)
                {
                    knockbackOrigin = inflictor.GlobalPosition;
                }

                knockbackDir = knockbackOrigin.DirectionTo(GlobalPosition);
            }

            ApplyImpulse(knockback.)
            */
        }
    }
}
