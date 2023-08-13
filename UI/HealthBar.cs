using Godot;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.UI;

public partial class HealthBar : Control
{
    public TextureProgressBar ProgressBar { get; set; }

    public override void _Ready()
    {
        ProgressBar = GetNode<TextureProgressBar>("TextureProgressBar");
        this.GetEventBus().PlayerHealthChanged += (args) =>
        {
            ProgressBar.Value = args.NewHealth;
        };
    }
}
