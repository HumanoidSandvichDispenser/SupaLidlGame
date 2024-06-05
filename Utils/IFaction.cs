using Godot;

namespace SupaLidlGame.Utils;

[System.Flags]
public enum FactionName
{
    Player = 1,
    Doc = 2,
}

public interface IFaction
{
    /// <summary>
    /// The faction index that this entity belongs to.
    /// </summary>
    [Export]
    public FactionName Faction { get; set; }

    public bool AlignsWith(IFaction other)
    {
        return (Faction & other.Faction) > 0;
    }

    public bool AlignsFullyWith(IFaction other)
    {
        return Faction == other.Faction;
    }
}
