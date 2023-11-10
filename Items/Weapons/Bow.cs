using Godot;

namespace SupaLidlGame.Items.Weapons;

public partial class Bow : ProjectileSpawner
{
    protected bool _isOnFire = false;

    protected Area2D _ignitionArea;

    protected override void SpawnProjectile(Scenes.Map map,
        Vector2 direction, float velocityModifier = 1)
    {
        base.SpawnProjectile(map, direction, velocityModifier);
    }

    public override void _Ready()
    {
        base._Ready();
        _ignitionArea = GetNode<Area2D>("IgnitionArea");
        _ignitionArea.AreaEntered += (Area2D area) =>
        {
            _isOnFire = false;
        };
    }
}
