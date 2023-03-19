using Godot;
using SupaLidlGame.Characters;

namespace SupaLidlGame.Items
{
    public abstract partial class Item : Node2D
    {
        [Export]
        public string ItemName { get; set; }

        [Export]
        public string Description { get; set; }

        [Export]
        public int StackSize { get; set; }

        public int Count { get; set; } = 1;

        public Character CharacterOwner { get; set; }

        /// <summary>
        /// Determines if this item can stack with other items
        /// </summary>
        public virtual bool StacksWith(Item item)
        {
            if (ItemName != item.ItemName)
            {
                return false;
            }

            if (Count + item.Count <= StackSize)
            {
                return true;
            }

            // several more conditions may be added soon

            return false;
        }

        public abstract void Equip(Character character);

        public abstract void Unequip(Character character);

        public abstract void Use();

        public abstract void Deuse();
    }
}
