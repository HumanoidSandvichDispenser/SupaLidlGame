using Godot;

namespace SupaLidlGame.State.Character
{
    public abstract partial class CharacterState : Node, IState<CharacterState>
    {
        [Export]
        public Characters.Character Character { get; set; }

        public virtual IState<CharacterState> Enter(IState<CharacterState> prev) => null;

        public virtual void Exit(IState<CharacterState> next)
        {

        }

        public virtual CharacterState Process(double delta)
        {
            if (Character.StunTime > 0)
            {
                Character.StunTime -= delta;
            }
            return null;
        }

        public virtual CharacterState PhysicsProcess(double delta)
        {
            Character.Velocity = Character.NetImpulse;

            if (Character.NetImpulse.LengthSquared() < Mathf.Pow(Character.Speed, 2))
            {
                Character.Velocity += Character.Direction.Normalized()
                    * Character.Speed;
                // we should only modify velocity that is in the player's control
                Character.ModifyVelocity();
            }

            Character.NetImpulse = Character.NetImpulse.MoveToward(
                Vector2.Zero,
                (float)delta * Character.Speed * Character.Friction);

            Character.MoveAndSlide();

            return null;
        }

        public virtual CharacterState Input(InputEvent @event) => null;
    }
}
