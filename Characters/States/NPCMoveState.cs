using Godot;

namespace SupaLidlGame.Characters.State
{
    public partial class NPCMoveState : NPCState
    {
        [Export]
        public CharacterState IdleState { get; set; }

        public override CharacterState Process(double delta)
        {
            base.Process(delta);
            if (Character.Direction.LengthSquared() == 0)
            {
                return IdleState;
            }
            return null;
        }
    }
}
