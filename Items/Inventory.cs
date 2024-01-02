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
        set => EquipItem(value);
    }

    private int _selectedIndex;

    public int SelectedIndex
    {
        get => _selectedIndex;
        set => EquipIndex(value);
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

        base._Ready();
    }

    public virtual bool EquipIndex(int index)
    {
        if (index >= Hotbar.Count)
        {
            return false;
        }

        _selectedItem?.Unequip(Character);
        _selectedIndex = index;

        if (index >= 0)
        {
            _selectedItem = Hotbar[index];
            _selectedItem?.Equip(Character);
        }
        else
        {
            _selectedItem = null;
        }

        GD.Print($"Inventory: {index} is new selected index.");

        return true;
    }

    protected virtual bool EquipItem(Item item)
    {
        if (item is null)
        {
            EquipIndex(-1);
        }

        int index = Hotbar.IndexOf(item);
        if (index < 0)
        {
            GD.PushWarning("Trying to equip item not in the hot inventory.");
        }
        return EquipIndex(index);
    }

    [System.Obsolete]
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
