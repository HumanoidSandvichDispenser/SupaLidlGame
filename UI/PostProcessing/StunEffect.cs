using Godot;

namespace SupaLidlGame.UI.PostProcessing;

public partial class StunEffect : ColorRect
{
    public override void _Ready()
    {
        Events.EventBus.Instance.PlayerStun += () =>
        {
            GetNode<AnimationPlayer>("AnimationPlayer").Play("tighten");
        };
    }
}
