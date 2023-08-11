using Godot;

namespace SupaLidlGame.State.Thinker;

public partial class ThinkerStateMachine : StateMachine<ThinkerState>
{
    [Export]
    public override ThinkerState InitialState { get; set; }

    private double _thinkTime = 0;

    public void Process(double delta)
    {
        var state = CurrentState.Process(delta);
        if (state is ThinkerState)
        {
            ChangeState(state);
        }

        if ((_thinkTime -= delta) <= 0)
        {
            _thinkTime = CurrentState.ThinkDelta;
            Think();
        }
    }

    public void PhysicsProcess(double delta)
    {
        var state = CurrentState.PhysicsProcess(delta);
        if (state is ThinkerState)
        {
            ChangeState(state);
        }
    }

    public void Think()
    {
        var state = CurrentState.Think();
        if (state is ThinkerState)
        {
            ChangeState(state);
        }
    }

    public override bool ChangeState(ThinkerState nextState)
    {
        if (base.ChangeState(nextState))
        {
            _thinkTime = CurrentState.ThinkDelta;
            return true;
        }
        return false;
    }
}
