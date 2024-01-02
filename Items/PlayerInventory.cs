namespace SupaLidlGame.Items;

public partial class PlayerInventory : Inventory
{
    public override bool EquipIndex(int index)
    {
        bool result = base.EquipIndex(index);

        if (result)
        {
            Events.EventBus.Instance.EmitSignal(
                Events.EventBus.SignalName.PlayerInventoryUpdate, this);
        }

        return result;
    }
}
