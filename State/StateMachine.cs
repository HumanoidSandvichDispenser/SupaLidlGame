using Godot;
using SupaLidlGame.Extensions;
using System.Collections.Generic;

namespace SupaLidlGame.State;

public abstract partial class StateMachine<T> : Node where T : Node, IState<T>
{
    [Signal]
    public delegate void StateChangedEventHandler(Node state);

    public T CurrentState { get; protected set; }

    public abstract T InitialState { get; set; }

    public override void _Ready()
    {
        ChangeState(InitialState);
    }

    public virtual bool ChangeState(T nextState)
    {
        return ChangeState(nextState, out Stack<T> _);
    }

    public bool ChangeState(T nextState, out T finalState)
    {
        var status = ChangeState(nextState, out Stack<T> states);
        finalState = states.Peek();
        return status;
    }

    public bool ChangeState(T nextState, out Stack<T> states)
    {
        states = new Stack<T>();
        states.Push(CurrentState);
        var result = ChangeStateRecursive(nextState, states);
        EmitSignal(SignalName.StateChanged, states.Peek());
        return result;
    }

    protected virtual bool ChangeStateRecursive(
        T nextState,
        Stack<T> states)
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
        states.Push(nextState);

        // if the next state decides it should enter a different state,
        // then we enter that different state instead
        var nextNextState = nextState.Enter(previousState);

        if (nextNextState is T t)
        {
            return ChangeStateRecursive(t, states);
        }

        return true;
    }

    /// <summary>
    /// Changes the current state to a state of type U which must inherit from T.
    /// </summary>
    public bool ChangeState<U>(out U state) where U : T
    {
        state = this.FindChildOfType<U>();
        return ChangeState(state);
    }

    public bool ChangeState(string name, out T state)
    {
        state = GetNode<T>(name);
        return ChangeState(state);
    }
}
