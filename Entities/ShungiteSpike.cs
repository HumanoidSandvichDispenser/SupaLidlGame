using Godot;
using GodotUtilities;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.Entities;

public partial class ShungiteSpike : Projectile
{
    [Export]
    public PackedScene Dart { get; set; }

    [Export]
    public BoundingBoxes.Hurtbox Hurtbox { get; set; }

    [Export]
    public AudioStreamPlayer2D HitSound { get; set; }

    [Export]
    public AnimationPlayer AnimationPlayer { get; set; }

    public Vector2 Target { get; set; }

    public double ExplodeTime { get; set; } = 2;

    public double FreezeTime
    {
        get => _freezeTime;
        set
        {
            _currentFreezeTime = _freezeTime = value;
        }
    }

    private double _currentExplodeTime;
    private double _freezeTime;
    private double _currentFreezeTime;
    private bool _hasFrozen = false;
    private bool _hasExploded = false;

    private Scenes.Map _map;

    public override void _Ready()
    {
        _currentFreezeTime = FreezeTime;
        _currentExplodeTime = ExplodeTime;
        _map = this.GetAncestor<Scenes.Map>();

        Hurtbox.ReceivedDamage += OnReceivedDamage;
        AnimationPlayer.Play("spin");

        AnimationPlayer.AnimationFinished += (StringName anim) =>
        {
            if (anim == "explode")
            {
                QueueFree();
            }
        };

        base._Ready();
    }

    private void OnReceivedDamage(
        float damage,
        Characters.Character inflictor,
        float knockback,
        Items.Weapon weapon,
        Vector2 knockbackDir)
    {
        // if we were hit by the player before the spike freezes,
        // spawn a dart towards where the player is aiming
        if (inflictor is Characters.Player player)
        {
            var dart = CreateDart(player.Target.Normalized());
            dart.Hitbox.Faction = player.Faction;
            Hitbox.IsDisabled = true;
        }

        HitSound.At(GlobalPosition).PlayOneShot();
        _hasExploded = true;
        _hasFrozen = true;
        AnimationPlayer.Stop();
        AnimationPlayer.Play("explode");
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
        if ((_currentFreezeTime -= delta) <= 0)
        {
            if (!_hasFrozen)
            {
                Speed = 0;
                _hasFrozen = true;
                AnimationPlayer.Stop();
            }

            if ((_currentExplodeTime -= delta) <= 0)
            {
                if (!_hasExploded)
                {
                    CreateDart(Vector2.Up);
                    CreateDart((Vector2.Up + Vector2.Right).Normalized());
                    CreateDart(Vector2.Right);
                    CreateDart((Vector2.Down + Vector2.Right).Normalized());
                    CreateDart(Vector2.Down);
                    CreateDart((Vector2.Down + Vector2.Left).Normalized());
                    CreateDart(Vector2.Left);
                    CreateDart((Vector2.Up + Vector2.Left).Normalized());
                    AnimationPlayer.Play("explode");
                    _hasExploded = true;
                }
            }
        }
    }
}
