using Godot;
using SupaLidlGame.Entities;

namespace SupaLidlGame.State.NPC.Doc;

public partial class DocUnwantedFrequencyState : DocShungiteSpikeState
{
    public override NPCState Enter(IState<NPCState> previous)
    {
        GetNode<AnimationPlayer>("../../Animations/Telegraph")
            .Play("unwanted_frequencies");
        NPC.ShouldMove = false;
        return base.Enter(previous);
    }

    public override void Exit(IState<NPCState> nextState)
    {
        GetNode<AnimationPlayer>("../../Animations/Telegraph").Stop();
        GetNode<GpuParticles2D>("../../Effects/UnwantedFrequenciesParticles")
            .Emitting = false;
        NPC.ShouldMove = true;
    }

    protected override Projectile SpawnProjectile(
        Vector2 position,
        Vector2 direction)
    {
        var projectile = _map.SpawnEntity<UnwantedFrequency>(Projectile);
        projectile.Hitbox.Faction = NPC.Faction;
        projectile.GlobalPosition = position;
        projectile.Direction = direction;
        projectile.Delay = 1.0 / Doc.Intensity;
        return projectile;
    }

    protected override void Attack()
    {
        var player = _world.CurrentPlayer;
        var playerPos = player.GlobalPosition;
        var docPos = NPC.GlobalPosition;
        var projectile = SpawnProjectile(docPos, docPos.DirectionTo(playerPos))
            as UnwantedFrequency;
        projectile.Homing = player;

        _currentAttackDuration = 1;
        _currentAttacks++;
    }
}
