using Godot;

namespace SupaLidlGame.Items.Weapons;

public partial class ProjectileSpawner : Ranged
{
    [Export]
    public PackedScene Projectile { get; set; }

    [Export]
    public bool ShouldOverrideProjectileDamage { get; set; } = true;

    [Export]
    public bool ShouldOverrideVelocity { get; set; } = true;

    [Export]
    public bool ShouldRotate { get; set; } = true;

    public override void Attack()
    {
        var map = Utils.World.Instance.CurrentMap;
        var projectile = map.SpawnEntity<Entities.Projectile>(Projectile);
        projectile.Hitbox.Faction = Character.Faction;
        projectile.Direction = Character.Target.Normalized();
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

        Character.Inventory.EmitSignal("UsedItem", this);
    }
}
