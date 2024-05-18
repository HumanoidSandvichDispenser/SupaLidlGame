using Godot;

namespace SupaLidlGame.Utils;

public interface IFaction
{
    /// <summary>
    /// The faction index that this entity belongs to.
    /// </summary>
    [Export]
    public ushort Faction { get; set; }

    public bool AlignsWith(IFaction other)
    {
        return (Faction & other.Faction) > 0;
    }

    public bool AlignsFullyWith(IFaction other)
    {
        return Faction == other.Faction;
    }
}
