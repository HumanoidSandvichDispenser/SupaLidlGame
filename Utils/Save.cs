using Godot;

namespace SupaLidlGame.Utils;

public partial class Save : Resource
{
    [Export]
    public State.Global.Progression Progression { get; set; }

    [Export]
    public State.Global.MapState MapState { get; set; }

    [Export]
    public State.Global.Stats Stats { get; set; }

    public Save()
    {
        Progression = new();
        MapState = new();
        Stats = new();
    }
}
