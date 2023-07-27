using Godot;

namespace SupaLidlGame.State.Character;

public partial class NPCIdleState : NPCState
{
    [Export]
    public CharacterState MoveState { get; set; }

    public override CharacterState Process(double delta)
    {
        base.Process(delta);
        if (Character.Direction.LengthSquared() > 0.01f)
        {
            return MoveState;
        }
        return null;
    }

    public override IState<CharacterState> Enter(IState<CharacterState> prev)
    {
        Character.MovementAnimation?.Play("idle");
        return base.Enter(prev);
    }
}
