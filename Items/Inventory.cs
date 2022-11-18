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

                _selectedItem.Equip(Character);
            }
        }

        public Item AddItem(Item item)
        {
            if (Items.Count >= MaxCapacity)
            {
                return null;
            }

            Items.Add(item);
            return item;
        }

        public Item DropItem(Item item)
        {
            throw new System.NotImplementedException();
        }

        public override void _Ready()
        {
            Owner = GetParent<Character>();
            foreach (Node child in GetChildren())
            {
                if (child is Item item)
                {
                    Items.Add(item);
                }
            }
            base._Ready();
        }
    }
}
