using Godot;

namespace SupaLidlGame.UI.PostProcessing;

public partial class Vignette : TextureRect
{
    public override void _Ready()
    {
        Events.EventBus.Instance.PlayerHurt += (Events.HurtArgs args) =>
        {
            GetNode<AnimationPlayer>("AnimationPlayer").Play("tighten");
        };
    }
}
