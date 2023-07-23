using Godot;

namespace SupaLidlGame.UI;

public partial class UIController : Control
{
    [Export]
    public TextureProgressBar PlayerHealthBar { get; set; }

    [Export]
    public BossBar BossBar { get; set; }
}
