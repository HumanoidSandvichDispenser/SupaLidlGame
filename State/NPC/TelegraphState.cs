using Godot;

namespace SupaLidlGame.State.NPC;

public partial class NPCTelegraphState : NPCState
{
    [Export]
    public double Duration { get; set; }

    public override NPCState Enter(IState<NPCState> previousState)
    {
        return null;
    }

    public override void Exit(IState<NPCState> nextState)
    {

    }
}
