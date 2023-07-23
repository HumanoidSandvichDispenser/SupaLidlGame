using Godot;

namespace SupaLidlGame.Characters;

public abstract partial class Boss : Enemy
{
    [Export]
    public State.NPC.NPCStateMachine BossStateMachine { get; set; }

    public abstract int Intensity { get; }

    [Export]
    public bool IsActive { get; set; }
}
