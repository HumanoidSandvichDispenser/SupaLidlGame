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

    private bool _hasPlayedExitAnim = false;

    private bool _previouslyCouldAttack = false;

    public override NPCState Enter(IState<NPCState> previousState)
    {
        _currentDuration = Duration;
        NPC.ShouldMove = false;
        _hasPlayedExitAnim = false;
        return null;
    }

    public override void Exit(IState<NPCState> nextState)
    {
        NPC.CanAttack = _previouslyCouldAttack;
    }

    public override NPCState Process(double delta)
    {
        if (!NPC.Inventory.IsUsingItem)
        {
            if (!_hasPlayedExitAnim)
            {
                _hasPlayedExitAnim = true;
                _previouslyCouldAttack = NPC.CanAttack;
                NPC.CanAttack = false;
                TelegraphAnimationPlayer.Play("exit_out");
            }

            if ((_currentDuration -= delta) <= 0)
            {
                return TelegraphState;
            }
        }
        return null;
    }
}
