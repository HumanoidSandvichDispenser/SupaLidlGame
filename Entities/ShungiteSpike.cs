using Godot;
using GodotUtilities;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.Entities;

public partial class ShungiteSpike : Projectile
{
    [Export]
    public PackedScene Dart { get; set; }

    [Export]
    public double ExplodeTime { get; set; } = 6;

    [Export]
    public BoundingBoxes.Hurtbox Hurtbox { get; set; }

    [Export]
    public AudioStreamPlayer2D HitSound { get; set; }

    [Export]
    public AnimationPlayer AnimationPlayer { get; set; }

    private double _currentExplodeTime;

    private Scenes.Map _map;

    public override void _Ready()
    {
        _currentExplodeTime = ExplodeTime;
        _map = this.GetAncestor<Scenes.Map>();

        Hurtbox.ReceivedDamage += OnReceivedDamage;
        AnimationPlayer.Play("spin");

        base._Ready();
    }

    private void OnReceivedDamage(
        float damage,
        Characters.Character inflictor,
        float knockback,
        Vector2 knockbackOrigin = default,
        Vector2 knockbackVector = default)
    {
        HitSound.At(GlobalPosition).PlayOneShot();
        QueueFree();
    }


    private Entities.ShungiteDart CreateDart(Vector2 direction)
    {
        var dart = _map.SpawnEntity<Entities.ShungiteDart>(Dart);
        dart.Direction = direction;
        dart.Hitbox.Faction = Hitbox.Faction;
        dart.GlobalPosition = GlobalPosition;
        dart.Delay = 0;
        return dart;
    }

    public override void _Process(double delta)
    {
        if ((_currentExplodeTime -= delta) <= 0)
        {
            CreateDart(Vector2.Up);
            CreateDart(Vector2.Down);
            CreateDart(Vector2.Left);
            CreateDart(Vector2.Right);
            QueueFree();
        }
    }
}
