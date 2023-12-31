using Godot;
using SupaLidlGame.Characters;
using Godot.Collections;

namespace SupaLidlGame.Items;

public partial class Inventory : Node2D
{
    public Character Character { get; private set; }

    [Export]
    public Array<Item> Hotbar { get; private set; }

    [Export]
    public Array<ItemMetadata> Items { get; private set; }

    [Export]
    public Dictionary<string, int> InventoryMap { get; set; }

    [Signal]
    public delegate void UsedItemEventHandler(Item item);

    public const int MaxCapacity = 3;

    private Item _selectedItem;

    public Item SelectedItem
    {
        get => _selectedItem;
        set => EquipItem(value, ref _selectedItem);
    }

    private int _quickSwitchIndex = -1;

    public const int QUICKSWITCH_SIZE = 3;

    public int CurrentQuickSwitchIndex
    {
        get => _quickSwitchIndex;
        set
        {
            const int size = QUICKSWITCH_SIZE;
            _quickSwitchIndex = (value % size + size) % size;
        }
    }

    public bool IsUsingItem => SelectedItem?.IsUsing ?? false;

    public Inventory()
    {
        //InventoryMap = new Dictionary<string, int>();
        //InventoryMap.Add("equip_1", 0);
        //InventoryMap.Add("equip_2", 1);
        //InventoryMap.Add("equip_3", 2);
    }

    public override void _Ready()
    {
        if (Hotbar is null)
        {
            // instantiating a new array will prevent characters from
            // sharing inventories
            Hotbar = new();
        }

        if (Items is null)
        {
            Items = new();
        }

        Character = GetParent<Character>();
        foreach (Node child in GetChildren())
        {
            if (child is Item item)
            {
                AddItem(item);
            }
        }

        Events.EventBus.Instance.EmitSignal(
            Events.EventBus.SignalName.PlayerInventoryUpdate, this);
        base._Ready();
    }

    public bool EquipIndex(int index)
    {
        if (index < Hotbar.Count)
        {
            return EquipItem(Hotbar[index], ref _selectedItem);
        }

        return EquipItem(null, ref _selectedItem);
    }

    private bool EquipItem(Item item, ref Item slot)
    {
        if (item is not null)
        {
            if (!Hotbar.Contains(item))
            {
                GD.PrintErr("Tried to equip an item not in the inventory.");
                return false;
            }
        }

        if (slot is not null)
        {
            slot.Unequip(Character);
        }

        slot = item;

        if (item is not null)
        {
            item.Equip(Character);
        }

        Events.EventBus.Instance.EmitSignal(
            Events.EventBus.SignalName.PlayerInventoryUpdate, this);

        return true;
    }

    public Item GetItemByMap(string keymap)
    {
        if (InventoryMap.ContainsKey(keymap))
        {
            int idx = InventoryMap[keymap];
            if (idx < Hotbar.Count)
            {
                return Hotbar[InventoryMap[keymap]];
            }
        }
        else GD.Print(keymap + " does not exist");
        return null;
    }

    public Item AddItemToHotbar(ItemMetadata metadata)
    {
        var item = metadata.Instance.Instantiate<Item>();
        AddItem(item);
        AddChild(item);
        GD.Print("Added " + item.Metadata.Name);
        return item;
    }

    public Item AddItem(Item item)
    {
        if (Hotbar.Count >= MaxCapacity)
        {
            return null;
        }

        item.CharacterOwner = Character;
        item.Visible = false;
        if (!Hotbar.Contains(item))
        {
            Hotbar.Add(item);
        }
        return item;
    }

    public Item DropItem(Item item)
    {
        item.CharacterOwner = null;
        item.Visible = true;
        var e = SelectedItem = item;
        throw new System.NotImplementedException();
    }
}
