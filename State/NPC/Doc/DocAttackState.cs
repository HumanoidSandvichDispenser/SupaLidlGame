using Godot;
using GodotUtilities;

namespace SupaLidlGame.State.NPC.Doc;

public abstract partial class DocAttackState : NPCState
{
    public abstract double Duration { get; set; }

    public abstract double AttackDuration { get; set; }

    public abstract PackedScene Projectile { get; set; }

    public abstract DocChooseAttackState ChooseAttackState { get; set; }

    protected Scenes.Map _map;
    protected Utils.World _world;
    protected Characters.Doc _doc;
    protected double _currentDuration = 0;
    protected double _currentAttackDuration = 0;

    public override void _Ready()
    {
        _doc = NPC as Characters.Doc;
    }

    public override NPCState Enter(IState<NPCState> previousState)
    {
        _map = this.GetAncestor<Scenes.Map>();
        _world = this.GetAncestor<Utils.World>();
        _currentDuration = Duration;
        _currentAttackDuration = AttackDuration;
        return null;
    }

    protected abstract void Attack();
}
