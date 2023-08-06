using Godot;

namespace SupaLidlGame.State.Character.Animation;

public abstract partial class AnimationState : Node, IState<AnimationState>
{
    [Export]
    public AnimationPlayer AnimationPlayer { get; set; }

    //[Export]
    //public 

    public virtual IState<AnimationState> Enter(IState<AnimationState> prev) => null;

    //public bool AdvanceCondition

    public virtual void Exit(IState<AnimationState> next)
    {

    }

    public virtual AnimationState Process(double delta)
    {
        return null;
    }

    public virtual AnimationState PhysicsProcess(double delta)
    {
        return null;
    }

    public virtual AnimationState Input(InputEvent @event) => null;
}
