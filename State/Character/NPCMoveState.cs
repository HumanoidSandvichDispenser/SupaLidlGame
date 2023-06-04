using Godot;

namespace SupaLidlGame.State.Character;

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

    public override IState<CharacterState> Enter(IState<CharacterState> prev)
    {
        Character.Sprite.Play("move");
        return base.Enter(prev);
    }
}
