using Godot;

namespace SupaLidlGame.State.Weapon
{
    public partial class RangedIdleState : WeaponState
    {
        [Export]
        public RangedFireState FireState { get; set; }

        [Export]
        public Items.Weapons.Ranged Weapon { get; set; }

        public override IState<WeaponState> Enter(IState<WeaponState> prev)
        {
            Weapon.Visible = !Weapon.ShouldHideIdle;
            return null;
        }

        public override WeaponState Use()
        {
            return FireState;
        }

        public override void Exit(IState<WeaponState> nextState)
        {
            Weapon.Visible = true;
        }
    }
}
