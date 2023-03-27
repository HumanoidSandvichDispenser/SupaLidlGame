using System.Collections.Generic;
using Godot;
using SupaLidlGame.Characters;

namespace SupaLidlGame.Items
{
    public partial class Inventory : Node2D
    {
        public Character Character { get; private set; }

        [Export]
        public Godot.Collections.Array<ItemInfo> Items { get; private set; }

        [Export]
        public ItemInfo Test { get; set; }

        public const int MaxCapacity = 32;

        private Item _primaryItem;

        private Item _offhandItem;

        public Item PrimaryItem
        {
            get => _primaryItem;
            private set => _primaryItem = value;
        }

        public Item OffhandItem
        {
            get => _offhandItem;
            private set => _offhandItem = value;
        }

        public Inventory()
        {
            Items = new Godot.Collections.Array<ItemInfo>();
        }

        public bool EquipItem(ItemInfo info, bool offhand = false)
        {
            if (UnequipItem() == info)
            {
                // if the item that was unequipped was our item, then we stop
                return true;
            }

            var idx = Items.IndexOf(info);

            if (idx < 0)
            {
                return false;
            }

            ItemInfo item = Items[idx];

            string name = offhand ? "Offhand" : "Primary";
            PrimaryItem = item.InstantiateItem(name);
            PrimaryItem.Equip(Character);
            PrimaryItem.CharacterOwner = Character;
            //PrimaryItem.CharacterOwner
            AddChild(PrimaryItem);
            return true;
        }

        public ItemInfo UnequipItem(bool offhand = false)
        {
            Item item;
            if (!offhand)
            {
                item = Character.Inventory.GetNode<Item>("Primary");
            }
            else
            {
                item = Character.Inventory.GetNode<Item>("Offhand");
            }

            if (item is null)
            {
                return null;
            }

            item.QueueFree();
            return item.Info;
        }

        /*
        private bool EquipItem(Item item, ref Item slot)
        {
            if (item is not null && item.Info.IsOneHanded)
            {
                // we can not equip this if either hand is occupied by
                // two-handed item

                if (_selectedItem is not null && !_selectedItem.Info.IsOneHanded)
                {
                    return false;
                }

                if (_offhandItem is not null && !_offhandItem.Info.IsOneHanded)
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
        */

        public ItemInfo AddItem(ItemInfo info)
        {
            if (Items.Count >= MaxCapacity)
            {
                return null;
            }

            info.CharacterOwner = Character;
            //item.Visible = false;
            Items.Add(info);
            return info;
        }

        public Item DropItem(ItemInfo item)
        {
            item.CharacterOwner = null;
            //item.Visible = true;
            //var e = SelectedItem = item;
            throw new System.NotImplementedException();
        }

        public override void _Ready()
        {
            Character = GetParent<Character>();
            EnsureChildrenItems();
            base._Ready();
        }

        private void EnsureChildrenItems()
        {
            foreach (Node child in GetChildren())
            {
                if (child is Item item)
                {
                    GD.Print(item.Info.Count);
                    if (Items.IndexOf(item.Info) < 0)
                    {
                        AddItem(item.Info);
                        PrimaryItem = item;
                    }
                }
            }
        }

        public void SelectFirstItem()
        {
            EnsureChildrenItems();
            if (Items.Count > 0)
            {
                EquipItem(Items[0]);
            }
        }
    }
}
