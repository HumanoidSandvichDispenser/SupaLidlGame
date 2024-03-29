using Godot;

namespace SupaLidlGame.State.Weapon;

public partial class SwordAttackState : WeaponState
{
    [Export]
    public SupaLidlGame.Items.Weapons.Sword Sword { get; set; }

    [Export]
    public SwordIdleState IdleState { get; set; }

    [Export]
    public bool HasAlternateAnimation { get; set; } = false;

    private double _attackDuration = 0;

    private double _useDuration = 0;

    private double _attackAnimDuration = 0;

    private bool _isAlternate = false;

    public override WeaponState Enter(IState<WeaponState> prevState)
    {
        Sword.EnableParry();
        Sword.Attack();

        if (HasAlternateAnimation && _isAlternate)
        {
            Sword.AnimationPlayer.Play("attack_alternate");
        }
        else
        {
            Sword.AnimationPlayer.Play("attack");
        }

        _attackDuration = Sword.AttackTime;
        _useDuration = Sword.UseTime;
        _attackAnimDuration = Sword.AttackAnimationDuration;

        Sword.Visible = true;
        Sword.UseDirection = Sword.Character.Target;
        return null;
    }

    public override void Exit(IState<WeaponState> nextState)
    {
        Sword.Deattack();
    }

    public override WeaponState Use()
    {
        /*
        if (_useDuration <= 0)
        {
            // if we are still playing the current attack animation, we should alternate
            if (_attackAnimDuration > 0)
            {
                _isAlternate = !_isAlternate;
            }
            return IdleState;
        }
        */

        return null;
    }

    public override WeaponState Process(double delta)
    {
        if (_attackDuration > 0)
        {
            if ((_attackDuration -= delta) <= 0)
            {
                Sword.Deattack();
            }
        }

        if ((_attackAnimDuration -= delta) <= 0)
        {
            Sword.AnimationPlayer.Play("RESET");
            _isAlternate = false;
            return IdleState;
        }

        _useDuration -= delta;

        return null;
    }
}
