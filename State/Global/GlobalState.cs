using Godot;
using SupaLidlGame.Utils;

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

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
    }

    public void ImportFromSave(Save save)
    {
        Progression = save.Progression;
        MapState = save.MapState;
        Stats = save.Stats;
    }

    public void ExportToSave(Save save)
    {
        save.Progression = Progression;
        save.MapState = MapState;
        save.Stats = Stats;
    }
}
