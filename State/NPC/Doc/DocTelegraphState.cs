using Godot;

namespace SupaLidlGame.State.NPC.Doc;

public partial class DocTelegraphState : NPCState
{
    [Export]
    public AnimationPlayer TelegraphAnimationPlayer { get; set; }

    [Export]
    public DocChooseAttackState AttackState { get; set; }

    [Export]
    public double Duration { get; set; } = 1;

    private double _currentDuration = 1;

    public override NPCState Enter(IState<NPCState> previousState)
    {
        _currentDuration = Duration;
        TelegraphAnimationPlayer.Play("enter_in");
        float randX = GD.RandRange(-128, 128);
        float randY = GD.RandRange(-128, 128);
        NPC.GlobalPosition = new Vector2(randX, randY);
        return null;
    }

    public override void Exit(IState<NPCState> nextState)
    {

    }

    public override NPCState Process(double delta)
    {
        if ((_currentDuration -= delta) <= 0)
        {
            return AttackState;
        }
        return null;
    }
}
