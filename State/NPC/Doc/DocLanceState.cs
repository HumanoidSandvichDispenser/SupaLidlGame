using Godot;

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

        //if (_doc.Intensity > 2)
        //{
        //    if (_doc.Inventory.SelectedItem != _doc.LanceHold)
        //    {
        //        _doc.Inventory.SelectedItem = _doc.LanceHold;
        //        _doc.UseCurrentItem();
        //        _doc.CanAttack = false;
        //    }
        //}
        //else
        //{
        //    // wtf are we doing here?
        //    throw new System.InvalidOperationException();
        //}

        //_oldFriction = _doc.Friction;
        //_doc.Friction = 0;

        //Attack();

        return state;
    }

    public override void Exit(IState<NPCState> nextState)
    {
        //_doc.Friction = _oldFriction;
        //_doc.ApplyImpulse(Vector2.Zero, true);
        //_doc.DeuseCurrentItem();
        //_doc.CanAttack = true;
        base.Exit(nextState);
    }

    protected override void Attack()
    {
        //var pos = _doc.GlobalPosition;
        //var player = _world.CurrentPlayer;
        //var playerPos = player.GlobalPosition;
        //var predictedPos = Utils.Physics.PredictNewPosition(
        //    pos, DashSpeed, playerPos, player.Velocity, out float time);
        //var dir = _doc.GlobalPosition.DirectionTo(predictedPos);

        //_currentAttackDuration = AttackDuration = time;

        //_doc.ApplyImpulse(dir * DashSpeed, true);
    }

    public override NPCState Process(double delta)
    {
        return null;
    }
}
