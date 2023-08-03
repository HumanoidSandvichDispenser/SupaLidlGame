using Godot;
using SupaLidlGame.Entities;

namespace SupaLidlGame.State.NPC.Doc;

public partial class DocShungiteSpikeState : DocShungiteDartState
{
    protected int _currentAttacks = 0;

    public override NPCState Enter(IState<NPCState> previous)
    {
        Doc.CanAttack = false;
        if (this is not DocUnwantedFrequencyState)
        {
            Doc.TelegraphAnimation.Play("shungite_spike");
        }
        _currentAttacks = 0;
        _currentAttackDuration = 1;
        Doc.ShouldMove = false;
        Doc.CanAttack = false;
        return base.Enter(previous);
    }

    public override void Exit(IState<NPCState> nextState)
    {
        Doc.ShouldMove = true;
        Doc.CanAttack = true;
        //Doc.TelegraphAnimation.Stop();
        //Doc.TelegraphAnimation.Stop();
    }

    protected override Projectile SpawnProjectile(
        Vector2 position,
        Vector2 direction)
    {
        Doc.TelegraphAnimation.Play("shungite_spike");
        var projectile = base.SpawnProjectile(position, direction)
            as ShungiteSpike;
        projectile.GlobalRotation = 0;
        projectile.Delay = 0;
        projectile.Hitbox.Faction = Doc.Faction;
        if (projectile.Hurtbox is not null)
        {
            projectile.Hurtbox.Faction = Doc.Faction;
        }
        return projectile;
    }

    protected override void Attack()
    {
        var player = NPC.FindBestTarget();
        if (player is null)
        {
            return;
        }
        var playerPos = player.GlobalPosition;
        var docPos = NPC.GlobalPosition;
        var projectile = SpawnProjectile(docPos, Vector2.Zero) as ShungiteSpike;
        var projectileSpeed = projectile.Speed = 96;

        // predict to player's position
        var targetPos = Utils.Physics.PredictNewPosition(
            docPos,
            projectileSpeed,
            playerPos,
            player.Velocity,
            out float time);
        projectile.Direction = projectile.GlobalPosition.DirectionTo(targetPos);
        projectile.FreezeTime = time + 0.25; // freeze when it reaches target

        _currentAttackDuration = 1;
        _currentAttacks++;
    }

    public override NPCState Process(double delta)
    {
        if (Doc.StunTime > 0)
        {
            return null;
        }

        if ((_currentAttackDuration -= delta) <= 0 || Doc.Intensity > 2)
        {
            Attack();
        }

        if (_currentAttacks >= Doc.Intensity)
        {
            return ChooseAttackState;
        }

        return null;
    }
}
