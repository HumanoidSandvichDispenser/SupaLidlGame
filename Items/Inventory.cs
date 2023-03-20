using System.Collections.Generic;
using Godot;
using SupaLidlGame.Characters;

namespace SupaLidlGame.Items
{
    public partial class Inventory : Node2D
    {
        public Character Character { get; private set; }

        public List<Item> Items { get; private set; } = new List<Item>();

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

        private bool EquipItem(Item item, ref Item slot)
        {
            if (item is not null && item.IsOneHanded)
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

        public Item AddItem(Item item)
        {
            if (Items.Count >= MaxCapacity)
            {
                return null;
            }

            item.CharacterOwner = Character;
            item.Visible = false;
            Items.Add(item);
            return item;
        }

        public Item DropItem(Item item)
        {
            item.CharacterOwner = null;
            item.Visible = true;
            var e = SelectedItem = item;
            throw new System.NotImplementedException();
        }

        public override void _Ready()
        {
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
    }
}
