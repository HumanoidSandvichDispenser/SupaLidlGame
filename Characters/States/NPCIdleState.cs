using Godot;

namespace SupaLidlGame.Characters.State
{
    public partial class NPCIdleState : NPCState
    {
        [Export]
        public CharacterState MoveState { get; set; }

        public override CharacterState Process(double delta)
        {
            base.Process(delta);
            if (Character.Direction.LengthSquared() > 0)
            {
                return MoveState;
            }
            return null;
        }

        public override CharacterState Enter(CharacterState previousState)
        {
            Character.Sprite.Play("idle");
            return base.Enter(previousState);
        }
    }
}
