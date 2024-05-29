using Godot;
using SupaLidlGame.Items;

namespace SupaLidlGame.UI.Inventory;

public partial class InventoryGrid : GridContainer
{
    private SupaLidlGame.Items.Inventory _inventorySource;

    public SupaLidlGame.Items.Inventory InventorySource
    {
        set
        {
            _inventorySource = value;
        }
        get => _inventorySource;
    }

    public override void _Ready()
    {

    }

    public void Redraw()
    {

    }
}
