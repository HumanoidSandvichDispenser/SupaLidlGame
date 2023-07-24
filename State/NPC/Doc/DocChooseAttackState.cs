using Godot;
using System.Linq;
using System.Collections.Generic;

namespace SupaLidlGame.State.NPC.Doc;

public partial class DocChooseAttackState : NPCState
{
    [Export]
    public DocShungiteDartState DartState { get; set; }

    [Export]
    public DocShungiteSpikeState SpikeState { get; set; }

    [Export]
    public DocUnwantedFrequencyState UnwantedFrequencyState { get; set; }

    [Export]
    public DocLanceState LanceState { get; set; }

    [Export]
    public DocExitState ExitState { get; set; }

    public Characters.Doc Doc => NPC as Characters.Doc;

    private HashSet<NPCState> _possibleStates = new HashSet<NPCState>();

    private int _consecutiveAttacks = 0;

    public override void _Ready()
    {
        ResetStates();
        base._Ready();
    }

    private void ResetStates()
    {
        _possibleStates.Add(DartState);
        _possibleStates.Add(SpikeState);
        _possibleStates.Add(UnwantedFrequencyState);
    }

    public override NPCState Enter(IState<NPCState> previous)
    {
        if (Doc.Intensity == 3)
        {
            return LanceState;
        }

        if (previous is not DocTelegraphState)
        {
            _consecutiveAttacks++;
        }
        else
        {
            _consecutiveAttacks = 1;
        }

        if (_consecutiveAttacks > Doc.Intensity)
        {
            _consecutiveAttacks = 1;
            ResetStates();
            return ExitState;
        }

        if (_possibleStates.Count == 0)
        {
            ResetStates();
        }

        // choose random attack
        var random = new System.Random();
        int index = random.Next(_possibleStates.Count);
        var state = _possibleStates.ElementAt(index);
        _possibleStates.Remove(state);
        return state;
    }
}
