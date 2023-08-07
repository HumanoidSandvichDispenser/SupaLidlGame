using Godot;

namespace SupaLidlGame.State.Global;

public partial class GlobalState : Node
{
    [Export]
    public Progression Progression { get; set; } = new();

    [Export]
    public MapState MapState { get; set; } = new();

    [Export]
    public Stats Stats { get; set; } = new();

    [Signal]
    public delegate void SummonBossEventHandler(string bossName);

    public void Print(string str) => GD.Print(str);
}
