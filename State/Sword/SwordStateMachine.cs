using Godot;

namespace SupaLidlGame.State.Sword
{
    public partial class SwordStateMachine : StateMachine<SwordState>
    {
        [Export]
        public override SwordState InitialState { get; set; }

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
            if (state is SwordState s)
            {
                ChangeState(s);
            }
        }
    }
}
