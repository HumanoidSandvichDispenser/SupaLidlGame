using Godot;

namespace SupaLidlGame.Items;

[GlobalClass]
public partial class ItemMetadata : Resource
{
    [Export]
    public Utils.ScenePath Instance { get; set; }

    [Export]
    public Texture2D Icon { get; set; }

    [Export]
    public string Name { get; set; }

    [Export(PropertyHint.MultilineText)]
    public string Description { get; set; }

    [Export]
    public int BuyPrice { get; set; }

    [Export]
    public int SellPrice { get; set; }
}
