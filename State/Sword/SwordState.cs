using Godot;

namespace SupaLidlGame.State.Sword
{
    public abstract partial class SwordState : Node, IState<SwordState>
    {
        public virtual SwordState Use() => null;

        public abstract IState<SwordState> Enter(IState<SwordState> previousState);

        public virtual void Exit(IState<SwordState> previousState)
        {

        }

        public virtual IState<SwordState> Process(double delta) => null;
    }
}
