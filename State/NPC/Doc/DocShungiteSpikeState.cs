using Godot;
using GodotUtilities;
using SupaLidlGame.Entities;

namespace SupaLidlGame.State.NPC.Doc;

public partial class DocShungiteSpikeState : DocAttackState
{
    private float _intensity = 1;

    protected override Projectile SpawnProjectile(
        Vector2 position,
        Vector2 direction)
    {
        var projectile = base.SpawnProjectile(position, direction);
        projectile.Delay = 4;
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
        _currentAttackDuration = float.PositiveInfinity;
    }
}
