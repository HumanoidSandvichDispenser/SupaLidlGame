using Godot;
using Godot.Collections;

namespace SupaLidlGame.State.Global;

public partial class Progression : Resource
{
    public Dictionary<string, bool> BossStatus { get; set; }
}
