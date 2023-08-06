using Godot;

namespace SupaLidlGame.State.Character.Animation;

public partial class AnimationStateMachine : StateMachine<AnimationState>
{
    [Export]
    public override AnimationState InitialState { get; set; }

    [Export]
    public AnimationPlayer Character { get; set; }

    public void Process(double delta)
    {
        var state = CurrentState.Process(delta);
        if (state is AnimationState)
        {
            ChangeState(state);
        }
    }

    public void PhysicsProcess(double delta)
    {
        var state = CurrentState.PhysicsProcess(delta);
        if (state is AnimationState)
        {
            ChangeState(state);
        }
    }

    public void Input(InputEvent @event)
    {
        var state = CurrentState.Input(@event);
        if (state is AnimationState)
        {
            ChangeState(state);
        }
    }
}
