using Godot;
using SupaLidlGame.Entities;

namespace SupaLidlGame.State.NPC.Doc;

public partial class DocShungiteSpikeState : DocShungiteDartState
{
    private float _intensity = 1;

    public override NPCState Enter(IState<NPCState> previous)
    {
        // subtract from total state time by intensity
        Duration = _currentDuration = 9 - 2 * Doc.Intensity;
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
        projectile.ExplodeTime = 6 - 2 * Doc.Intensity;
        projectile.Hitbox.Faction = projectile.Hurtbox.Faction = Doc.Faction;
        return projectile;
    }

    protected override void Attack()
    {
        var player = _world.CurrentPlayer;
        var playerPos = player.GlobalPosition;
        Vector2 left = playerPos + Vector2.Left * 64;
        Vector2 right = playerPos + Vector2.Right * 64;
        Vector2 up = playerPos + Vector2.Up * 64;
        Vector2 down = playerPos + Vector2.Down * 64;
        SpawnProjectile(left, Vector2.Zero);
        SpawnProjectile(right, Vector2.Zero);
        SpawnProjectile(up, Vector2.Zero);
        SpawnProjectile(down, Vector2.Zero);

        // only attack once and stop (but keep in this state for 8 seconds)
        _currentAttackDuration += 8;
    }

    public override NPCState Process(double delta)
    {
        if ((_currentDuration -= delta) <= 0)
        {
            return ChooseAttackState;
        }

        if ((_currentAttackDuration -= delta) <= 0)
        {
            Attack();
        }

        return null;
    }
}
