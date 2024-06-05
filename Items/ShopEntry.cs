using Godot;

namespace SupaLidlGame.Items;

[GlobalClass]
public partial class ShopEntry : Resource
{
    public ShopEntry() : base()
    {

    }

    public ShopEntry(ItemMetadata item) : base()
    {
        Item = item;
    }

    [Export]
    public ItemMetadata Item { get; set; }

    [Export]
    public string MapStateCondition { get; set; }
}
