using Godot;

namespace SupaLidlGame.State.Global;

public partial class Stats : Resource
{
    [Export]
    public Vector2 SaveLocation { get; set; }

    [Export]
    public string SaveMapKey { get; set; }

    [Export]
    public int DeathCount { get; set; } = 0;
}
