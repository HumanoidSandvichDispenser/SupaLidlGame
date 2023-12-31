using Godot;
using SupaLidlGame.Items;

namespace SupaLidlGame.UI;

public partial class Hotbar : GridContainer
{
    [Export]
    private Godot.Collections.Array<InventorySlot> _slots;

    public override void _Ready()
    {
        Events.EventBus.Instance.PlayerInventoryUpdate += OnInventoryUpdate;
    }

    public void OnInventoryUpdate(Inventory inventory)
    {
        for (int i = 0; i < 3; i++)
        {
            var slot = _slots[i];
            slot.Item = inventory.Hotbar[i].Metadata;
            slot.IsSelected = inventory.SelectedItem == inventory.Hotbar[i];
            GD.Print(inventory.Hotbar[i].Metadata.Name);
        }
    }
}
