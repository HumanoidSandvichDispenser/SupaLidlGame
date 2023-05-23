using Godot;

namespace SupaLidlGame.State.Sword
{
    public partial class SwordAnticipateState : SwordState
    {
        [Export]
        public SupaLidlGame.Items.Weapons.Sword Sword { get; set; }
        
        [Export]
        public SwordAttackState AttackState { get; set; }

        private double _anticipateTime;

        public override SwordState Enter(IState<SwordState> prevState)
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

        public override SwordState Process(double delta)
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
