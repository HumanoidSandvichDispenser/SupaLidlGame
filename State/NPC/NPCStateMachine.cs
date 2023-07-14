using Godot;

namespace SupaLidlGame.State.NPC;

public partial class NPCStateMachine : StateMachine<NPCState>
{
    [Export]
    public override NPCState InitialState { get; set; }

    public void Process(double delta)
    {
        var state = CurrentState.Process(delta);
        if (state is NPCState s)
        {
            ChangeState(s);
        }
    }
}
