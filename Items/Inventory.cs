using Godot;
using SupaLidlGame.Characters;
using Godot.Collections;

namespace SupaLidlGame.Items
{
    public partial class Inventory : Node2D
    {
        public Character Character { get; private set; }

        [Export]
        public Array<Item> Items { get; private set; }

        [Export]
        public Dictionary<string, int> InventoryMap { get; set; }

        public const int MaxCapacity = 32;

        private Item _selectedItem;

        private Item _offhandItem;

        public Item SelectedItem
        {
            get => _selectedItem;
            set => EquipItem(value, ref _selectedItem);
        }

        public Item OffhandItem
        {
            get => _selectedItem;
            set => EquipItem(value, ref _offhandItem);
        }

        public bool IsUsingItem => (SelectedItem?.IsUsing ?? false) ||
                (OffhandItem?.IsUsing ?? false);

        public Inventory()
        {
            InventoryMap = new Dictionary<string, int>();
            InventoryMap.Add("equip_1", 0);
            InventoryMap.Add("equip_2", 1);
            InventoryMap.Add("equip_3", 2);
        }

        public override void _Ready()
        {
            if (Items is null)
            {
                // instantiating a new array will prevent characters from
                // sharing inventories
                Items = new Array<Item>();
            }
            Character = GetParent<Character>();
            foreach (Node child in GetChildren())
            {
                if (child is Item item)
                {
                    GD.Print("Adding item " + item.Name);
                    AddItem(item);
                }
            }
            base._Ready();
        }

        private bool EquipItem(Item item, ref Item slot)
        {
            if (item is not null)
            {
                if (item.IsOneHanded)
                {
                    // we can not equip this if either hand is occupied by
                    // two-handed item

                    if (_selectedItem is not null && !_selectedItem.IsOneHanded)
                    {
                        return false;
                    }

                    if (_offhandItem is not null && !_offhandItem.IsOneHanded)
                    {
                        return false;
                    }
                }

                if (!Items.Contains(item))
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

            return true;
        }

        public Item GetItemByMap(string keymap)
        {
            if (InventoryMap.ContainsKey(keymap))
            {
                int idx = InventoryMap[keymap];
                if (idx < Items.Count)
                {
                    return Items[InventoryMap[keymap]];
                }
            }
            else GD.Print(keymap + " does not exist");
            return null;
        }

        public Item AddItem(Item item)
        {
            if (Items.Count >= MaxCapacity)
            {
                return null;
            }

            item.CharacterOwner = Character;
            item.Visible = false;
            if (!Items.Contains(item))
            {
                Items.Add(item);
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
}
