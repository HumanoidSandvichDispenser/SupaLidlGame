using Godot;

namespace SupaLidlGame.Characters;

public partial class Doc : Enemy
{
    [Export]
    public State.NPC.NPCStateMachine BossStateMachine { get; set; }

    public override void _Ready()
    {
        GD.Print(Health);
        base._Ready();
    }
    
    public override void _Process(double delta)
    {
        BossStateMachine.Process(delta);
        base._Process(delta);
    }
}