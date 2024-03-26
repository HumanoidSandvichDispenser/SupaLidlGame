using Godot;

namespace SupaLidlGame.Utils;

public partial class CharacterStats : Node
{
    public DoubleValue StaggerDamage { get; set; }

    [Export]
    public double StaggerCoefficient { get; set; } = 0.5;

    [Export]
    public double StaggerDecayVelocity { get; set; } = 5;

    [Export]
    public double MaxStagger { get; set; } = 25;

    [Signal]
    public delegate void StaggerEventHandler(double time);

    public bool ShouldStagger => StaggerDamage.Value >= MaxStagger;

    protected bool _shouldDecayStagger;

    protected Timer _staggerDecayTimer;

    public override void _Ready()
    {
        StaggerDamage = GetNode<DoubleValue>("StaggerDamage");

        if (StaggerDamage is null)
        {
            StaggerDamage = new DoubleValue();
            AddChild(StaggerDamage);
        }

        _staggerDecayTimer = new Timer();
        _staggerDecayTimer.Timeout += () => _shouldDecayStagger = true;
        _staggerDecayTimer.Stop();
        AddChild(_staggerDecayTimer);
    }

    public void AddStaggerDamage(float damage)
    {
        StaggerDamage.Value += damage * StaggerCoefficient;
        if (StaggerDamage.Value >= MaxStagger)
        {
            EmitSignal(SignalName.Stagger, 1);
        }
        else
        {
            _shouldDecayStagger = false;
            _staggerDecayTimer.Stop();
            _staggerDecayTimer.Start(1);
        }
    }

    public override void _Process(double delta)
    {
        if (_shouldDecayStagger)
        {
            StaggerDamage.Value = Mathf.MoveToward(
                StaggerDamage.Value, 0, StaggerDecayVelocity * delta);
        }
    }
}
