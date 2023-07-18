using Godot;

namespace SupaLidlGame.State.NPC.Doc;

public partial class DocExitState : NPCState
{
    [Export]
    public AnimationPlayer TelegraphAnimationPlayer { get; set; }

    [Export]
    public DocTelegraphState TelegraphState { get; set; }

    [Export]
    public double Duration { get; set; } = 1;

    private double _currentDuration = 0;

    public override NPCState Enter(IState<NPCState> previousState)
    {
        _currentDuration = Duration;
        TelegraphAnimationPlayer.Play("exit_out");
        return null;
    }

    public override void Exit(IState<NPCState> nextState)
    {

    }

    public override NPCState Process(double delta)
    {
        if ((_currentDuration -= delta) <= 0)
        {
            return TelegraphState;
        }
        return null;
    }
}
