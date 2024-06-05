using Godot;
using SupaLidlGame.Utils;

namespace SupaLidlGame.BoundingBoxes;

public abstract partial class BoundingBox : Area2D, IFaction
{
    [Export(PropertyHint.Flags)]
    public FactionName Faction { get; set; }
}
