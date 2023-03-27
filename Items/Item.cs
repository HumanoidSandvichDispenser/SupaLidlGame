using Godot;
using SupaLidlGame.Characters;

namespace SupaLidlGame.Items
{
    public abstract partial class Item : Node2D
    {
        [Export]
        public ItemInfo Info { get; set; }

        public Character CharacterOwner { get; set; }

        /// <summary>
        /// Determines if this item can directly stack with other items
        /// </summary>
        public virtual bool StacksWith(Item item)
        {
            if (!Info.CanStack)
            {
                return false;
            }

            if (Info.ItemName != item.Info.ItemName)
            {
                return false;
            }

            // several more conditions may be added soon

            return true;
        }

        public abstract void Equip(Character character);

        public abstract void Unequip(Character character);

        public abstract void Use();

        public abstract void Deuse();
    }
}
