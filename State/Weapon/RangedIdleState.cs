using Godot;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.State.Weapon;

public partial class RangedIdleState : WeaponState
{
    [Export]
    public WeaponState FireState { get; set; }

    [Export]
    public WeaponState AltFireState { get; set; }

    [Export]
    public Items.Weapons.Ranged Weapon { get; set; }

    [Export]
    public AnimationPlayer AnimationPlayer { get; set; }

    [Export]
    public string AnimationKey { get; set; }


    public override IState<WeaponState> Enter(IState<WeaponState> prev)
    {
        Weapon.Visible = !Weapon.ShouldHideIdle;
        AnimationPlayer?.TryPlay(AnimationKey);
        return null;
    }

    public override WeaponState Use()
    {
        return FireState;
    }

    public override WeaponState UseAlt()
    {
        GD.Print("Alt fire");
        return AltFireState;
    }

    public override void Exit(IState<WeaponState> nextState)
    {
        Weapon.Visible = true;
    }
}
