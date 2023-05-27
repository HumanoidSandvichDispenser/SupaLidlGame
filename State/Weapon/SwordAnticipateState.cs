using Godot;

namespace SupaLidlGame.State.Weapon
{
    public partial class SwordAnticipateState : WeaponState
    {
        [Export]
        public SupaLidlGame.Items.Weapons.Sword Sword { get; set; }
        
        [Export]
        public SwordAttackState AttackState { get; set; }

        private double _anticipateTime;

        public override WeaponState Enter(IState<WeaponState> prevState)
        {
            Sword.EnableParry();

            if (Sword.Character is SupaLidlGame.Characters.Player)
            {
                return AttackState;
            }

            if (Sword.Anchor.Rotation > Mathf.DegToRad(50))
            {
                Sword.AnimationPlayer.Play("anticipate_alternate");
            }
            else
            {
                Sword.AnimationPlayer.Play("anticipate");
            }
            _anticipateTime = Sword.NPCAnticipateTime;
            return null;
        }

        public override WeaponState Process(double delta)
        {
            // go into attack state if anticipation time is delta
            if ((_anticipateTime -= delta) <= 0)
            {
                return AttackState;
            }
            return null;
        }
    }
}
