using Godot;
using SupaLidlGame.Entities;

namespace SupaLidlGame.State.NPC.Doc;

public partial class DocShungiteSpikeState : DocShungiteDartState
{
    protected int _currentAttacks = 0;

    public override NPCState Enter(IState<NPCState> previous)
    {
        _currentAttacks = 0;
        _currentAttackDuration = 1;
        return base.Enter(previous);
    }

    protected override Projectile SpawnProjectile(
        Vector2 position,
        Vector2 direction)
    {
        var projectile = base.SpawnProjectile(position, direction)
            as ShungiteSpike;
        projectile.GlobalRotation = 0;
        projectile.Delay = 0;
        projectile.Hitbox.Faction = projectile.Hurtbox.Faction = Doc.Faction;
        return projectile;
    }

    protected override void Attack()
    {
        var player = _world.CurrentPlayer;
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
        GD.Print("projectile velocity: " + projectile.Velocity);

        _currentAttackDuration = 1;
        _currentAttacks++;
    }

    public override NPCState Process(double delta)
    {
        if ((_currentAttackDuration -= delta) <= 0)
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