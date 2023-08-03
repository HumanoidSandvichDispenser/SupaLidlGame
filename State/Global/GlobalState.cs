using Godot;

namespace SupaLidlGame.State.Global;

public partial class GlobalState : Node
{
    //public Utils.World World { get; set; }

    public Progression Progression { get; set; }

    [Signal]
    public delegate void SummonBossEventHandler(string bossName);
}
