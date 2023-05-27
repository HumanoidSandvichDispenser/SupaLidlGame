using Godot;

namespace SupaLidlGame.State.Weapon
{
    public abstract partial class WeaponState : Node, IState<WeaponState>
    {
        public virtual WeaponState Use() => null;

        public virtual WeaponState Deuse() => null;

        public abstract IState<WeaponState> Enter(IState<WeaponState> previousState);

        public virtual void Exit(IState<WeaponState> nextState)
        {

        }

        public virtual IState<WeaponState> Process(double delta) => null;
    }
}
