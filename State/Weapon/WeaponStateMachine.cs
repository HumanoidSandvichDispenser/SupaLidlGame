using Godot;

namespace SupaLidlGame.State.Weapon
{
    public partial class WeaponStateMachine : StateMachine<WeaponState>
    {
        [Export]
        public override WeaponState InitialState { get; set; }

        public void Use()
        {
            var state = CurrentState.Use();
            if (state is not null)
            {
                ChangeState(state);
            }
        }

        public void Process(double delta)
        {
            var state = CurrentState.Process(delta);
            if (state is WeaponState s)
            {
                ChangeState(s);
            }
        }
    }
}
