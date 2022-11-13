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

        public virtual CharacterState Process(double delta) => null;

        public virtual CharacterState PhysicsProcess(double delta)
        {
            Character.Velocity = Character.Direction * Character.Speed;
            Character.MoveAndSlide();
            return null;
        }

        public virtual CharacterState Input(InputEvent @event) => null;
    }
}
