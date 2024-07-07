using Godot;

namespace SupaLidlGame.Items.Weapons;

public partial class Bow : ProjectileSpawner
{
    protected bool _isOnFire = false;

    protected Area2D _ignitionArea;

    protected override Entities.Projectile SpawnProjectile(Scenes.Map map,
        Vector2 direction, float velocityModifier = 1)
    {
        var projectile = base.SpawnProjectile(map, direction, velocityModifier);

        if (_isOnFire)
        {
            GetNode<AnimatedSprite2D>("%Flame").Visible = false;
            // TODO: instead of doing 1.5x damage, create an "On Fire" debuff
            projectile.Hitbox.Damage *= 1.5f;
            _isOnFire = false;
        }

        return projectile;
    }

    public override void _Ready()
    {
        base._Ready();
        _ignitionArea = GetNode<Area2D>("IgnitionArea");
        var onAreaEntered = (Area2D area) =>
        {
            GetNode<AnimatedSprite2D>("%Flame").Visible = true;
            _isOnFire = true;
        };
        _ignitionArea.Connect(
            Area2D.SignalName.AreaEntered,
            Callable.From(onAreaEntered));
    }
}
