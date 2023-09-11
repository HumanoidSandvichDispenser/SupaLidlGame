using Godot;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.State.Weapon;

public partial class RangedChargeState : WeaponState
{
    [Export]
    public Items.Weapons.Ranged Weapon { get; set; }

    [Export]
    public RangedFireState FireState { get; set; }

    [Export]
    public RangedIdleState IdleState { get; set; }

    [Export]
    public AnimationPlayer AnimationPlayer { get; set; }

    [Export]
    public string AnimationKey { get; set; }

    private double _timeLeftToCharge = 0;

    //private double _timeLeftToOvercharge = 0;

    public override IState<WeaponState> Enter(IState<WeaponState> prev)
    {
        _timeLeftToCharge = Weapon.ChargeTime;
        AnimationPlayer?.TryPlay(AnimationKey);
        return null;
    }

    public override WeaponState Process(double delta)
    {
        if (_timeLeftToCharge > 0)
        {
            if ((_timeLeftToCharge -= delta) < 0)
            {
                _timeLeftToCharge = 0;
            }
        }

        Weapon.UseDirection = Weapon.Character.Target;
        GD.Print(Weapon.UseDirection);

        return null;
    }

    public override WeaponState Deuse()
    {
        // fire
        double progress = _timeLeftToCharge / Weapon.ChargeTime;

        if (progress > 0.5)
        {
            GD.Print("not enough");
            return IdleState;
        }

        GD.Print("firing");
        FireState.VelocityModifier = (float)(1 - progress);
        return FireState;
    }
}
