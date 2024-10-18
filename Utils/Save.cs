using Godot;

namespace SupaLidlGame.Utils;

public partial class Save : Resource, ISave
{
    [Export]
    public State.Global.Progression Progression { get; set; }

    [Export]
    public State.Global.MapState MapState { get; set; }

    [Export]
    public State.Global.Stats Stats { get; set; }

    [Export]
    public ulong TimeElapsed { get; set; }

    public Save()
    {
        Progression = new();
        MapState = new();
        Stats = new();
        TimeElapsed = 0;
    }
}
