using Godot;

namespace SupaLidlGame.State.Character;

public partial class CharacterStateMachine : StateMachine<CharacterState>
{
    [Export]
    public override CharacterState InitialState { get; set; }

    [Export]
    public Characters.Character Character { get; set; }

    public void Process(double delta)
    {
        var state = CurrentState.Process(delta);
        if (state is CharacterState)
        {
            ChangeState(state);
        }
    }

    public void PhysicsProcess(double delta)
    {
        var state = CurrentState.PhysicsProcess(delta);
        if (state is CharacterState)
        {
            ChangeState(state);
        }
    }

    public void Input(InputEvent @event)
    {
        var state = CurrentState.Input(@event);
        if (state is CharacterState)
        {
            ChangeState(state);
        }
    }
}
