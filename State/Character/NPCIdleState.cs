using Godot;

namespace SupaLidlGame.State.Character;

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

    public override IState<CharacterState> Enter(IState<CharacterState> previousState)
    {
        Character.MovementAnimation.Play("idle");
        return base.Enter(previousState);
    }
}
