using Godot;
using SupaLidlGame.Extensions;

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
        // TODO: clean this up
        if (!(NPC as Characters.Doc).IsActive)
        {
            return null;
        }

        _currentDuration = Duration;
        TelegraphAnimationPlayer.Play("enter_in");
        NPC.ShouldMove = true;

        var player = this.GetWorld().CurrentPlayer;
        Vector2 randVec;

        do
        {
            float randX = GD.RandRange(-112, 112);
            float randY = GD.RandRange(-112, 112);
            randVec = new Vector2(randX, randY);
        }
        while (randVec.DistanceSquaredTo(player.GlobalPosition) < 9216);
        // can not teleport within 96 units of the player

        NPC.GlobalPosition = randVec;
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
