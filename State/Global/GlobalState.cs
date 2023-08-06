using Godot;

namespace SupaLidlGame.State.Global;

public partial class GlobalState : Node
{
    //public Utils.World World { get; set; }

    [Export]
    public Progression Progression { get; set; } = new();

    [Export]
    public MapState MapState { get; set; } = new();

    [Signal]
    public delegate void SummonBossEventHandler(string bossName);
}
