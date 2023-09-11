using Godot;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.State.Weapon;

public partial class RangedFireState : WeaponState
{
    [Export]
    public Items.Weapons.Ranged Weapon { get; set; }

    [Export]
    public RangedIdleState IdleState { get; set; }

    [Export]
    public AnimationPlayer AnimationPlayer { get; set; }

    [Export]
    public string AnimationKey { get; set; }

    public float VelocityModifier { get; set; }

    private double _timeLeft = 0;

    public override IState<WeaponState> Enter(IState<WeaponState> prev)
    {
        //_timeLeft
        _timeLeft = Weapon.UseTime;
        Weapon.Attack(VelocityModifier);
        Weapon.UseDirection = Weapon.Character.Target;
        AnimationPlayer?.TryPlay(AnimationKey);
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
