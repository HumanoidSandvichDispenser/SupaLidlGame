using Godot;

namespace SupaLidlGame.State;

public abstract partial class StateMachine<T> : Node where T : Node, IState<T>
{
    public T CurrentState { get; protected set; }

    public abstract T InitialState { get; set; }

    public override void _Ready()
    {
        ChangeState(InitialState);
    }

    public virtual bool ChangeState(T nextState, bool isProxied = false)
    {
        if (nextState is null)
        {
            return false;
        }

        // NOTE: proxied states can call Exit() more than once
        if (CurrentState is not null)
        {
            CurrentState.Exit(nextState);
        }

        var previousState = CurrentState;
        CurrentState = nextState;

        // if the next state decides it should enter a different state,
        // then we enter that different state instead
        var nextNextState = nextState.Enter(previousState);

        if (nextNextState is T t)
        {
            return ChangeState(t, true);
        }

        return true;
    }
}
