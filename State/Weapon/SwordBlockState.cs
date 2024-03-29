using Godot;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.State.Weapon;

public partial class SwordBlockState : WeaponState
{
    [Export]
    public SupaLidlGame.Items.Weapons.Sword Sword { get; set; }

    [Export]
    public SwordIdleState IdleState { get; set; }

    [Export]
    public WeaponState UseState { get; set; }

    [Export]
    public string BlockAnimKey { get; set; }

    public bool HasBlocked { get; set; }

    private double _attackDuration = 0;

    private double _useDuration = 0;

    private bool _isAlternate = false;

    private float _oldBlockForce;

    public override WeaponState Enter(IState<WeaponState> prevState)
    {
        Sword.EnableParry(ulong.MaxValue);
        _oldBlockForce = Sword.BlockForce;
        Sword.BlockForce *= 4;

        _useDuration = Sword.UseAltTime;
        _attackDuration = Sword.AttackAltTime;

        Sword.UseDirection = Sword.Character.Target;

        Sword.AnimationPlayer.TryPlay(BlockAnimKey);

        return null;
    }

    public override WeaponState Use()
    {
        if (_attackDuration <= 0 && HasBlocked)
        {
            return UseState;
        }
        return null;
    }

    public void Deattack()
    {
        Sword.DisableParry();
    }

    public override void Exit(IState<WeaponState> nextState)
    {
        if (nextState == IdleState)
        {
            Sword.AnimationPlayer.Play("RESET");
        }

        Sword.BlockForce = _oldBlockForce;
        Deattack();
        HasBlocked = false;
    }

    public override WeaponState Process(double delta)
    {
        if (_attackDuration > 0)
        {
            if ((_attackDuration -= delta) <= 0)
            {
                Deattack();
            }
        }

        if ((_useDuration -= delta) <= 0)
        {
            return IdleState;
        }

        return null;
    }
}
