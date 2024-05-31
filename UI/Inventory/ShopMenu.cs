using Godot;

namespace SupaLidlGame.UI.Inventory;

public partial class ShopMenu : Control, IModal
{
    private Items.IItemCollection<Items.ShopEntry> _source;

    public Items.IItemCollection<Items.ShopEntry> Source
    {
        get => _source;
        set
        {
            GD.Print("Set ShopMenu source");
            _source = value;
            _inventoryGrid.Source = value;
        }
    }

    [Export]
    private InventoryGrid _inventoryGrid;

    public void HideModal()
    {
        Hide();
        _source = null;
    }

    public override void _Ready()
    {
        Events.EventBus.Instance.EnterShop += (string path) =>
        {
            var shop = ResourceLoader.Load<Items.Shop>(path);
            GD.Print("Loaded shop");
            Source = shop;
        };
    }
}
