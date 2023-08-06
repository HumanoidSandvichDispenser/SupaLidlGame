using Godot;

namespace SupaLidlGame.State.Thinker;

public abstract partial class ThinkerState : Node, IState<ThinkerState>
{
    [Export]
    public double ThinkDelta { get; set; } = 0.125;

    [Export]
    public Characters.NPC NPC { get; set; }

    public virtual IState<ThinkerState> Enter(IState<ThinkerState> prev) => null;

    public virtual void Exit(IState<ThinkerState> next)
    {

    }

    public virtual ThinkerState Process(double delta) => null;

    public virtual ThinkerState PhysicsProcess(double delta) => null;

    public virtual ThinkerState Think() => null;
}
