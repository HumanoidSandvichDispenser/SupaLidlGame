using Godot;

namespace SupaLidlGame.UI.PostProcessing;

public partial class StunEffect : ColorRect
{
    public override void _Ready()
    {
        Events.EventBus.Instance.PlayerStun += () =>
        {
            GD.Print("PLAYER STUNNED!!!");
            GetNode<AnimationPlayer>("AnimationPlayer").Play("tighten");
        };
    }
}
