using Godot;

namespace SupaLidlGame.State.Health;

public abstract partial class HealthState : Node, IState<HealthState>
{
    public virtual IState<HealthState> Enter(IState<HealthState> prev) => null;

    public virtual void Exit(IState<HealthState> next)
    {

    }
}
