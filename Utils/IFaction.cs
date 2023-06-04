using Godot;

namespace SupaLidlGame.Utils;

public interface IFaction
{
    /// <summary>
    /// The faction index that this entity belongs to.
    /// </summary>
    [Export]
    public ushort Faction { get; set; }
}
