using Godot;

namespace SupaLidlGame.State.NPC.Doc;

public abstract partial class DocAttackState : NPCState
{
    public abstract double Duration { get; set; }

    public abstract double AttackDuration { get; set; }

    public abstract PackedScene Projectile { get; set; }

    public abstract DocChooseAttackState ChooseAttackState { get; set; }

    protected abstract void Attack();
}
