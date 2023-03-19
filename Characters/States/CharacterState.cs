using Godot;

namespace SupaLidlGame.Characters.State
{
    public partial class CharacterState : Node
    {
        public Character Character { get; set; }

        /// <summary>
        /// Called when the <c>Character</c> enters this <c>CharacterState</c>.
        /// </summary>
        /// <remarks>
        /// This returns a <c>CharacterState</c> in case a state is being
        /// transitioned to but wants to transition to another state. For
        /// example, an attack state can return to an idle state, but that idle
        /// state can override it to the move state immediately when necessary.
        /// </remarks>
        public virtual CharacterState Enter(CharacterState previousState) => null;

        /// <summary>
        /// Called when the <c>Character</c> exits this <c>CharacterState</c>.
        /// </summary>
        public virtual void Exit(CharacterState nextState) { }

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
