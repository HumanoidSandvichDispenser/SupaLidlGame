using Godot;

namespace SupaLidlGame.Items.Weapons;

public partial class ProjectileSpawner : Ranged
{
    [Export]
    public PackedScene Projectile { get; set; }

    [ExportGroup("Projectile Overrides")]
    [Export]
    public bool ShouldOverrideProjectileDamage { get; set; } = true;

    [Export]
    public bool ShouldOverrideVelocity { get; set; } = true;

    [Export]
    public bool ShouldRotate { get; set; } = true;

    [ExportGroup("Multishot")]
    [Export]
    public int ProjectileCount { get; set; } = 1;

    [Export]
    public float ProjectileAngleDeviation { get; set; }

    private void SpawnProjectile(Scenes.Map map, Vector2 direction)
    {
        var projectile = map.SpawnEntity<Entities.Projectile>(Projectile);
        projectile.Hitbox.Faction = Character.Faction;
        projectile.Direction = direction;
        projectile.GlobalPosition = GlobalPosition;

        if (ShouldOverrideVelocity)
        {
            projectile.Speed = InitialVelocity;
        }

        if (ShouldRotate)
        {
            projectile.GlobalRotation = projectile.Direction.Angle();
        }

        if (ShouldOverrideProjectileDamage)
        {
            if (projectile.Hitbox is not null)
            {
                projectile.Hitbox.Damage = Damage;
            }
        }

        if (projectile is Utils.ITarget target)
        {
            if (Character is Characters.NPC npc)
            {
                target.CharacterTarget = npc.FindBestTarget();
            }
        }

    }

    public override void Attack()
    {
        Character.Inventory.EmitSignal("UsedItem", this);

        var map = Utils.World.Instance.CurrentMap;

        Vector2 target = Character.Target.Normalized();

        // avoid unnecessary math if only spawning 1 projectile
        if (ProjectileCount == 1)
        {
            SpawnProjectile(map, target);
            return;
        }

        // example: 4 projectiles =
        // i = 0 -> 1.5 theta
        // i = 1 -> 0.5 theta
        // i = 2 -> -0.5 theta
        // i = 3 -> -1.5 theta
        // i = x -> -x * 0.5 theta + max dev

        float theta = Mathf.DegToRad(ProjectileAngleDeviation);
        // maaax angle deviation = ((projectile count - 1) / 2) thetas
        float maxAngleDeviations = ((ProjectileCount - 1) / 2);
        for (int i = 0; i < ProjectileCount; i++)
        {
            float curDeviation = -i + maxAngleDeviations;
            GD.Print(curDeviation);
            SpawnProjectile(map, target.Rotated(curDeviation * theta));
        }
    }
}
