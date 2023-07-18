using Godot;
using System.Collections.Generic;

namespace SupaLidlGame.State.NPC.Doc;

public partial class DocChooseAttackState : NPCState
{
    [Export]
    public DocShungiteDartState DartState { get; set; }

    [Export]
    public DocShungiteSpikeState SpikeState { get; set; }

    [Export]
    public DocExitState ExitState { get; set; }

    public Characters.Doc Doc => NPC as Characters.Doc;

    private List<NPCState> _states = new List<NPCState>();

    private int _consecutiveAttacks = 0;

    public override void _Ready()
    {
        _states.Add(DartState);
        _states.Add(SpikeState);
        base._Ready();
    }

    public override NPCState Enter(IState<NPCState> previous)
    {
        if (previous is not DocTelegraphState)
        {
            _consecutiveAttacks++;
        }
        else
        {
            _consecutiveAttacks = 0;
        }

        if (_consecutiveAttacks > Doc.Intensity)
        {
            _consecutiveAttacks = 0;
            return ExitState;
        }

        // choose random attack
        var random = new System.Random();
        return _states[random.Next(_states.Count)];
    }
}
