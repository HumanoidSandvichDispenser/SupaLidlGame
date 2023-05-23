using Godot;

namespace SupaLidlGame.State.Sword
{
    public partial class SwordIdleState : SwordState
    {
        [Export]
        public SwordAnticipateState AnticipateState { get; set; }

        [Export]
        public SupaLidlGame.Items.Weapons.Sword Sword { get; set; }

        private double _attackCooldown;

        public override SwordState Enter(IState<SwordState> prevState)
        {
            if (prevState is SwordAttackState)
            {
                _attackCooldown = Sword.UseTime - Sword.AttackTime;
            }
            return null;
        }

        public override SwordState Use()
        {
            if (_attackCooldown <= 0)
            {
                return AnticipateState;
            }

            return AnticipateState;
        }

        public override SwordState Process(double delta)
        {
            if (_attackCooldown > 0)
            {
                _attackCooldown -= delta;
            }

            return null;
        }
    }
}
