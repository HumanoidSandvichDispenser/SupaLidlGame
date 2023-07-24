using Godot;
using Godot.Collections;

namespace SupaLidlGame.State.Global;

public partial class Progression : Resource
{
    public Dictionary<PackedScene, bool> BossStatus { get; set; }
}
