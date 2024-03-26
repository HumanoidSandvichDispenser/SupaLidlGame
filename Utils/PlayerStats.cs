using Godot;
using SupaLidlGame.Events;

namespace SupaLidlGame.Utils;

public partial class PlayerStats : CharacterStats
{
    public const int MAX_XP_PER_LEVEL = 4;

    public const int MAX_LEVELS = 4;

    public DoubleValue XP { get; set; }

    public IntValue Level { get; set; }

    public double XPDecayVelocity { get; set; } = 1;

    protected bool _shouldDecayXP = true;

    protected Timer _xpDecayTimer;

    public override void _Ready()
    {
        base._Ready();

        XP = GetNode<DoubleValue>("XP");
        Level = GetNode<IntValue>("Level");

        if (XP is null)
        {
            XP = new DoubleValue();
            AddChild(XP);
        }

        if (Level is null)
        {
            Level = new IntValue();
            AddChild(Level);
        }

        _xpDecayTimer = new Timer();
        _xpDecayTimer.Timeout += () => _shouldDecayXP = true;
        _xpDecayTimer.Stop();
        AddChild(_xpDecayTimer);

        var bus = EventBus.Instance;
        XP.Changed += (oldValue, newValue) =>
        {
            bus.EmitSignal(EventBus.SignalName.PlayerXPChanged, newValue);
        };

        Level.Changed += (oldValue, newValue) =>
        {
            bus.EmitSignal(EventBus.SignalName.PlayerLevelChanged, newValue);
        };

        bus.PlayerHit += (args) =>
        {
            double xp = XP.Value;
            xp += args.Weapon?.PlayerLevelGain ?? 0;

            if (xp >= MAX_XP_PER_LEVEL)
            {
                int deltaLevel = (int)(xp / MAX_XP_PER_LEVEL);
                xp %= MAX_XP_PER_LEVEL;
                Level.Value = Mathf.Min(Level.Value + deltaLevel, MAX_LEVELS);
            }

            if (Level.Value == MAX_LEVELS)
            {
                // if max level, can only go up to 1 xp
                xp = Mathf.Min(xp, 1);
            }

            XP.Value = xp;

            _shouldDecayXP = false;
            _xpDecayTimer.Start(1);
        };
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (_shouldDecayXP)
        {
            XP.Value = Mathf.MoveToward(XP.Value, 1, XPDecayVelocity * delta);
        }
    }
}
