using Godot;

namespace SupaLidlGame.Characters.State
{
    public partial class Machine : Node
    {
        protected Character _character;

        public CharacterState State { get; protected set; }

        [Export]
        public CharacterState InitialState { get; set; }

        [Export]
        public Character Character { get; set; }

        [Export]
        public int DebugLevel { get; set; }

        public override void _Ready()
        {
            ChangeState(InitialState);
        }

        public void ChangeState(CharacterState nextState, bool isProxied = false)
        {
            if (DebugLevel >= 2)
            {
                if (State is not null)
                {
                    string proxyNote = isProxied ? " (proxied)" : "";
                    GD.Print($"Transition{proxyNote} {State.Name} -> {nextState.Name}");
                }
            }

            if (DebugLevel >= 1)
            {
                if (GetNode<Label>("../Debug/State") is Label label)
                {
                    label.Text = nextState.Name;
                }
            }

            nextState.Character = Character;
            if (State != null)
            {
                State.Exit(nextState);
            }

            var nextNextState = nextState.Enter(State);

            State = nextState;

            if (nextNextState is not null)
            {
                ChangeState(nextNextState, true);
            }
        }

        public void Process(double delta)
        {
            CharacterState nextState = State.Process(delta);
            if (nextState is not null)
            {
                ChangeState(nextState);
            }
        }

        public void PhysicsProcess(double delta)
        {
            CharacterState nextState = State.PhysicsProcess(delta);
            if (nextState is not null)
            {
                ChangeState(nextState);
            }
        }

        public void Input(InputEvent @event)
        {
            CharacterState nextState = State.Input(@event);
            if (nextState is not null)
            {
                ChangeState(nextState);
            }
        }
    }
}
