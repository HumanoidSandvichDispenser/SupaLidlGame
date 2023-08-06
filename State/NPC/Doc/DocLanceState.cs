using Godot;
using SupaLidlGame.State.Thinker;

namespace SupaLidlGame.State.NPC.Doc;

public partial class DocLanceState : DocAttackState
{
    [Export]
    public float DashSpeed { get; set; } = 212;

    [Export]
    public DocExitState ExitState { get; set; }

    public override DocChooseAttackState ChooseAttackState
    {
        get => null;
        set { }
    }

    public override PackedScene Projectile
    {
        get => null;
        set { }
    }

    public override double AttackDuration { get; set; }

    public override double Duration { get; set; }

    protected Vector2 _dashDirection;

    protected double _dashTime;

    protected float _oldFriction = 0;

    public override NPCState Enter(IState<NPCState> previousState)
    {
        var state = base.Enter(previousState);
        _doc.ShouldMove = true;
        _doc.ThinkerStateMachine.ChangeState<DashDefensive>(out var _);

        return state;
    }

    protected override void Attack()
    {

    }

    public override NPCState Process(double delta)
    {
        return null;
    }
}
