using Godot;

namespace SupaLidlGame.State.Weapon;

public partial class RangedFireState : WeaponState
{
    [Export]
    public Items.Weapons.Ranged Weapon { get; set; }

    [Export]
    public RangedIdleState IdleState { get; set; }

    private double _timeLeft = 0;

    public override IState<WeaponState> Enter(IState<WeaponState> prev)
    {
        //_timeLeft
        _timeLeft = Weapon.UseTime;
        Weapon.Attack();
        Weapon.UseDirection = Weapon.Character.Target;
        return null;
    }

    public override WeaponState Process(double delta)
    {
        if ((_timeLeft -= delta) <= 0)
        {
            return IdleState;
        }
        return null;
    }

    public override void Exit(IState<WeaponState> nextState)
    {

    }
}
