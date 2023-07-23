using Godot;

namespace SupaLidlGame.UI;

public partial class HealthBar : Control
{
    public TextureProgressBar ProgressBar { get; set; }

    public override void _Ready()
    {
        ProgressBar = GetNode<TextureProgressBar>("TextureProgressBar");
    }
}
