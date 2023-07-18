using Godot;

namespace SupaLidlGame.State.NPC;

public abstract partial class NPCState : Node, IState<NPCState>
{
    [Export]
    public SupaLidlGame.Characters.NPC NPC { get; set; }

    public abstract IState<NPCState> Enter(IState<NPCState> previousState);

    public virtual void Exit(IState<NPCState> nextState)
    {

    }

    public virtual IState<NPCState> Process(double delta) => null;
}
