using Godot;
using GodotUtilities;
using SupaLidlGame.Entities;

namespace SupaLidlGame.State.NPC.Doc;

public partial class DocShungiteDartState : DocAttackState
{
    [Export]
    public override double Duration { get; set; }

    [Export]
    public override double AttackDuration { get; set; }

    [Export]
    public override PackedScene Projectile { get; set; }

    [Export]
    public override DocChooseAttackState ChooseAttackState { get; set; }

    [Export]
    public Characters.Doc Doc { get; set; }

    public override NPCState Enter(IState<NPCState> nextState)
    {
        return base.Enter(nextState);
    }

    protected virtual Projectile SpawnProjectile(
        Vector2 position,
        Vector2 direction)
    {
        var projectile = _map.SpawnEntity<Projectile>(Projectile);
        projectile.Hitbox.Faction = NPC.Faction;
        projectile.GlobalPosition = position;
        projectile.Direction = direction;
        projectile.GlobalRotation = direction.Angle();
        projectile.Delay = 1.0 / Doc.Intensity;
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
        // global position is (from npc to player) * 2 = (2 * npc) - player
        //projectile.GlobalPosition = 2 * NPC.GlobalPosition - playerPos;
        Vector2 position1 = 3 * NPC.GlobalPosition - 2 * playerPos;
        Vector2 position2 = 3 * playerPos - 2 * NPC.GlobalPosition;
        Vector2 direction1 = position1.DirectionTo(playerPos);
        Vector2 direction2 = -direction1;
        SpawnProjectile(position1, direction1);
        SpawnProjectile(position2, direction2);
        _currentAttackDuration = AttackDuration / Doc.Intensity;
    }

    public override NPCState Process(double delta)
    {
        if (Doc.StunTime > 0)
        {
            return null;
        }

        if ((_currentDuration -= delta) <= 0 || Doc.Intensity > 2)
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
