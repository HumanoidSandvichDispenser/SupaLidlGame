using Godot;

namespace SupaLidlGame.Items.Weapons;

public abstract partial class Ranged : Weapon
{
    [Export]
    public float CharacterRecoil { get; set; } = 0;

    [Export]
    public float AngleDeviation { get; set; }

    [Export]
    public float ChargeTime { get; set; }

    [Export]
    public State.Weapon.WeaponStateMachine StateMachine { get; set; }

    public override bool IsUsingPrimary => StateMachine.CurrentState
        is State.Weapon.RangedFireState or State.Weapon.RangedChargeState;

    public bool IsChargeable => ChargeTime > 0;

    public bool IsCharging { get; protected set; }

    public override void Use()
    {
        StateMachine.Use();
        base.Use();
    }

    public override void Deuse()
    {
        StateMachine.Deuse();
        base.Deuse();
    }

    public override void _Process(double delta)
    {
        StateMachine.Process(delta);
        base._Process(delta);
    }

    public abstract void Attack();

    public virtual void Attack(float velocityModifier) => Attack();
}
