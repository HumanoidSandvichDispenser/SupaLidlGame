using Godot;
using GodotUtilities;
using SupaLidlGame.Items;

namespace SupaLidlGame.UI.Inventory;

public partial class InventoryGrid : GridContainer
{
    private SupaLidlGame.Items.Inventory _inventorySource;

    [Export]
    private PackedScene _slotScene;

    public SupaLidlGame.Items.Inventory InventorySource
    {
        set
        {
            _inventorySource = value;
            Redraw();
        }
        get => _inventorySource;
    }

    public void Redraw()
    {
        if (_inventorySource is null)
        {
            this.QueueFreeChildren();
        }

        var children = GetChildren();

        for (int i = children.Count; i < _inventorySource.InventoryCapacity; i++)
        {
            AddChild(_slotScene.Instantiate<InventorySlot>());
        }

        for (int i = children.Count - 1; i >= _inventorySource.InventoryCapacity; i--)
        {
            children[i].QueueFree();
        }

        children = GetChildren();

        for (int i = 0; i < children.Count; i++)
        {
            InventorySlot slot = children[i] as InventorySlot;

            if (i >= _inventorySource.Items.Count)
            {
                slot.Item = null;
            }
            else if (slot.Item != _inventorySource.Items[i])
            {
                slot.Item = _inventorySource.Items[i];
            }
        }
    }
}
