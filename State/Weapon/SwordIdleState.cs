using Godot;

namespace SupaLidlGame.State.Weapon
{
    public partial class SwordIdleState : WeaponState
    {
        [Export]
        public SwordAnticipateState AnticipateState { get; set; }

        [Export]
        public SupaLidlGame.Items.Weapons.Sword Sword { get; set; }

        private double _attackCooldown;

        public override WeaponState Enter(IState<WeaponState> prevState)
        {
            if (prevState is SwordAttackState)
            {
                _attackCooldown = Sword.UseTime - Sword.AttackTime;
            }

            Sword.Visible = !Sword.ShouldHideIdle;

            return null;
        }

        public override void Exit(IState<WeaponState> nextState)
        {
            Sword.Visible = true;
        }

        public override WeaponState Use()
        {
            if (_attackCooldown <= 0)
            {
                return AnticipateState;
            }

            return AnticipateState;
        }

        public override WeaponState Process(double delta)
        {
            if (_attackCooldown > 0)
            {
                _attackCooldown -= delta;
            }

            return null;
        }
    }
}
