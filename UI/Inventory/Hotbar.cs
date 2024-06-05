using Godot;

namespace SupaLidlGame.UI.Inventory;

public partial class Hotbar : GridContainer
{
    [Export]
    private Godot.Collections.Array<InventorySlot> _slots;

    public override void _Ready()
    {
        Events.EventBus.Instance.PlayerInventoryUpdate += OnInventoryUpdate;
    }

    public void OnInventoryUpdate(Items.Inventory inventory)
    {
        GD.Print($"UPDATE: {inventory.SelectedIndex} is selected index.");
        for (int i = 0; i < 3; i++)
        {
            var slot = _slots[i];
            slot.Item = inventory.Hotbar[i]?.Metadata;
            slot.IsSelected = inventory.SelectedIndex == i;
        }
    }
}
