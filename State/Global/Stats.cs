using Godot;
using Godot.Collections;

namespace SupaLidlGame.State.Global;

[GlobalClass]
public partial class Stats : Resource
{
    [Export]
    public Vector2 SaveLocation { get; set; }

    [Export]
    public string SaveMapKey { get; set; }

    [Export]
    public int DeathCount { get; set; } = 0;

    [Export]
    public Array<Items.ItemMetadata> Items { get; set; } = new();
}
