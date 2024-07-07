using Godot;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.State.Weapon;

public partial class BowAltState : WeaponState
{
    [Export]
    public Items.Weapons.Bow Bow { get; set; }

    [Export]
    public RangedIdleState IdleState { get; set; }

    [Export]
    public AnimationPlayer AnimationPlayer { get; set; }

    [Export]
    public string AnimationKey { get; set; }

    [Export]
    public int MaxCount { get; set; } = 5;

    [Export]
    public float AngleDeviation { get; set; }

    public float VelocityModifier { get; set; } = 1;

    private Timer _timer;

    private int _count = 0;

    private float _oldDeviation;

    public override IState<WeaponState> Enter(IState<WeaponState> prev)
    {
        //_timer = GetTree().CreateTimer(Weapon.UseAltTime);
        _timer = new Timer();
        _count = 1;
        _oldDeviation = Bow.ProjectileAngleDeviation;
        Bow.AngleDeviation = AngleDeviation;

        var timeout = () =>
        {
            Bow.Attack(VelocityModifier);
            _count++;
        };

        Bow.Attack(VelocityModifier);
        Bow.UseDirection = Bow.Character.Target;
        AnimationPlayer?.TryPlay(AnimationKey);

        _timer.Connect(Timer.SignalName.Timeout, Callable.From(timeout));
        AddChild(_timer);
        _timer.Start(Bow.UseAltTime);

        GD.Print("Entered alt fire state");

        return null;
    }

    public override WeaponState Process(double delta)
    {
        if (_count >= MaxCount)
        {
            return IdleState;
        }
        return null;
    }

    public override void Exit(IState<WeaponState> nextState)
    {
        _timer.QueueFree();
        Bow.AngleDeviation = _oldDeviation;
    }
}
