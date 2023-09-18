using Godot;

namespace SupaLidlGame.State.Character;

public partial class PlayerMaxLevelState : PlayerState
{
    [Export]
    public float CastDistance { get; set; }

    [Export]
    public double CastTime { get; set; }

    [Export]
    public int LevelCost { get; set; }

    private double _castTimeLeft = 0;

    private static PackedScene _lightningScene;

    private GpuParticles2D _particles;

    static PlayerMaxLevelState()
    {
        _lightningScene = GD.Load<PackedScene>("res://Entities/Lightning.tscn");
    }

    public override void _Ready()
    {
        base._Ready();
        _particles = GetNode<GpuParticles2D>("%Effects/CastParticles");
    }

    public override IState<CharacterState> Enter(IState<CharacterState> prevState)
    {
        _player.AttackAnimation.Play("cast");
        _player.Inventory.Visible = false;
        _player.Stats.Level.Value -= LevelCost;
        _castTimeLeft = CastTime;
        return base.Enter(prevState);
    }

    public override void Exit(IState<CharacterState> nextState)
    {
        _player.Inventory.Visible = true;
        base.Exit(nextState);
    }

    public override CharacterState Process(double delta)
    {
        // return to idle when finished
        if ((_castTimeLeft -= delta) <= 0)
        {
            return IdleState;
        }

        return base.Process(delta);
    }

    public override CharacterState Input(InputEvent @event)
    {
        return null;
    }

    public void Cast()
    {
        Scenes.Map map = Utils.World.Instance.CurrentMap;
        bool hasSpawned = false;
        foreach (var node in map.Entities.GetChildren())
        {
            if (node is Characters.Character ch)
            {
                if (ch.Faction == Character.Faction)
                {
                    continue;
                }

                if (!ch.IsAlive)
                {
                    continue;
                }

                // check if distance good and spawn lightning
                var dist = Character.GlobalPosition
                    .DistanceTo(ch.GlobalPosition);
                if (dist <= CastDistance)
                {
                    // delay up to 150 milliseconds
                    double delay = GD.Randf() * 0.150;
                    GetTree().CreateTimer(delay).Timeout += () =>
                    {
                        SpawnLightning(ch);
                    };
                    hasSpawned = true;
                }
            }
        }

        if (hasSpawned)
        {
            _player.Camera.Shake(2.0f, 0.5f);
            // delay to time out
            // this is a workaround where particles of amount 1 won't render
            GetTree().CreateTimer(0.1f).Timeout += () =>
            {
                _particles.Emitting = false;
            };
            _particles.Emitting = true;
            //_particles.SetDeferred("emitting", false);
        }
    }

    private void SpawnLightning(Characters.Character target)
    {
        if (target is null || !IsInstanceValid(target) || !target.IsAlive)
        {
            return;
        }
        var map = Utils.World.Instance.CurrentMap;
        var instance = map.SpawnEntity<Entities.Lightning>(_lightningScene);
        instance.Hitbox.Faction = Character.Faction;
        instance.Hitbox.Inflictor = Character;
        instance.Weapon = null;
        instance.GlobalPosition = target.GlobalPosition;
    }
}
