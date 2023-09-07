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

    /// <summary>
    /// Changes the state of the <c>StateMachine</c>.
    /// </summary>
    /// <param name="nextState">The next state to transition to.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="nextState" /> is a
    /// valid state, otherwise <see langword="false" />
    /// </returns>
    public virtual bool ChangeState(T nextState)
    {
        return ChangeState(nextState, out Stack<T> _);
    }

    /// <summary>
    /// Changes the state of the <c>StateMachine</c>.
    /// </summary>
    /// <param name="nextState">The next state to transition to.</param>
    /// <param name="finalState">The actual state.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="nextState" /> is a
    /// valid state, otherwise <see langword="false" />
    /// </returns>
    public bool ChangeState(T nextState, out T finalState)
    {
        var status = ChangeState(nextState, out Stack<T> states);
        finalState = states.Peek();
        return status;
    }

    /// <summary>
    /// Changes the state of the <c>StateMachine</c>.
    /// </summary>
    /// <param name="nextState">The next state to transition to.</param>
    /// <param name="states">Stack of all states that transitioned/proxied.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="nextState" /> is a
    /// valid state, otherwise <see langword="false" />
    /// </returns>
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
    /// Changes the state of the <c>StateMachine</c> of type
    /// <typeparamref name="U" /> which must inherit from
    /// <typeparamref name="T" />.
    /// </summary>
    /// <typeparam name="U">The type of the state to transition to.</typeparam>
    /// <param name="state">The resulting state to be transitioned to.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="nextState" /> is a
    /// valid state, otherwise <see langword="false" />
    /// </returns>
    public bool ChangeState<U>(out U state) where U : T
    {
        state = this.FindChildOfType<U>();
        return ChangeState(state);
    }

    /// <summary>
    /// Changes the state of the <c>StateMachine</c> with node name
    /// <paramref name="name" />.
    /// </summary>
    /// <typeparam name="U">The type of the state to transition to.</typeparam>
    /// <param name="name">
    /// The name of the <typeparamref name="T" /> node.
    /// </param>
    /// <param name="state">The resulting state to be transitioned to.</param>
    /// <returns>
    /// <see langword="true" /> if <paramref name="nextState" /> is a
    /// valid state, otherwise <see langword="false" />
    /// </returns>
    public bool ChangeState(string name, out T state)
    {
        state = GetNode<T>(name);
        return ChangeState(state);
    }
}
