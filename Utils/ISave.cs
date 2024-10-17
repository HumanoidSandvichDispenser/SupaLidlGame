using Godot;

namespace SupaLidlGame.Utils;

public interface ISave
{
    public State.Global.Progression Progression { get; set; }

    public State.Global.MapState MapState { get; set; }

    public State.Global.Stats Stats { get; set; }
}
