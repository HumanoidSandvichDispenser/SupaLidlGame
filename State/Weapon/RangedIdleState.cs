using Godot;

namespace SupaLidlGame.State.Weapon
{
    public partial class RangedIdleState : WeaponState
    {
        [Export]
        public RangedFireState FireState { get; set; }

        public override IState<WeaponState> Enter(IState<WeaponState> prev) => null;

        public override WeaponState Use()
        {
            return FireState;
        }

        public override void Exit(IState<WeaponState> nextState)
        {

        }
    }
}
