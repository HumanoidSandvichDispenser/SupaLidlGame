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

        public Item SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (!Items.Contains(value))
                {
                    GD.PrintErr("Tried to equip an item not in the inventory.");
                    return;
                }

                if (_selectedItem is not null)
                {
                    _selectedItem.Unequip(Character);
                }

                _selectedItem = value;

                // this is to handle if item was manually unequipped
                if (_selectedItem is not null)
                {
                    _selectedItem.Equip(Character);
                }
            }
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
