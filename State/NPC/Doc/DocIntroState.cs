using Godot;

namespace SupaLidlGame.State.NPC.Doc;

public partial class DocIntroState : NPCState
{
    [Export]
    public NPCState NextState { get; set; }

    [Export]
    public double Duration { get; set; }

    private double _currentDuration;
    private Characters.Doc _doc;

    public override void _Ready()
    {
        base._Ready();
        _doc = NPC as Characters.Doc;
    }

    public override NPCState Enter(IState<NPCState> previousState)
    {
        _currentDuration = Duration;
        _doc.MiscAnimation.Play("intro");
        return null;
    }

    public override NPCState Process(double delta)
    {
        if ((_currentDuration -= delta) <= 0)
        {
            return NextState;
        }

        return null;
    }
}
