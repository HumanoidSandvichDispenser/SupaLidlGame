using Godot;

namespace SupaLidlGame.State.Sword
{
    public partial class SwordAttackState : SwordState
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

        public override SwordState Enter(IState<SwordState> prevState)
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

            GD.Print(_attackDuration);
            GD.Print(_useDuration);
            GD.Print(_attackAnimDuration);

            return null;
        }

        public override void Exit(IState<SwordState> nextState)
        {
            Sword.Deattack();
        }

        public override SwordState Use()
        {
            if (_useDuration <= 0)
            {
                // if we are still playing the current attack animation, we should alternate
                if (_attackAnimDuration > 0)
                {
                    _isAlternate = !_isAlternate;
                }
                return AnticipateState;
            }

            return null;
        }

        public override SwordState Process(double delta)
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
