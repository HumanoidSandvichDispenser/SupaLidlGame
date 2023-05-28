using Godot;

namespace SupaLidlGame.State.Weapon
{
    public partial class SwordAttackState : WeaponState
    {
        [Export]
        public SupaLidlGame.Items.Weapons.Sword Sword { get; set; }

        [Export]
        public SwordAnticipateState AnticipateState { get; set; }

        [Export]
        public SwordIdleState IdleState { get; set; }

        private double _attackDuration = 0;

        private double _useDuration = 0;

        private double _attackAnimDuration = 0;

        private bool _isAlternate = false;

        public override WeaponState Enter(IState<WeaponState> prevState)
        {
            //Sword.AnimationPlayer.Stop();
            Sword.Attack();

            if (_isAlternate)
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

            return null;
        }

        public override void Exit(IState<WeaponState> nextState)
        {
            Sword.Deattack();
        }

        public override WeaponState Use()
        {
            if (_useDuration <= 0)
            {
                // if we are still playing the current attack animation, we should alternate
                if (_attackAnimDuration > 0)
                {
                    _isAlternate = !_isAlternate;
                }
                return IdleState;
            }

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
}
