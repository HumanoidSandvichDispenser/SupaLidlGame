using Godot;
using SupaLidlGame.Characters;
using MonoCustomResourceRegistry;

namespace SupaLidlGame.Items
{
    [RegisteredType(nameof(ItemInfo))]
    public partial class ItemInfo : Resource
    {
        [Export]
        public string ItemName { get; set; } = "Item Name";

        [Export]
        public string Description { get; set; } = "Item description.";

        [Export]
        public bool CanStack { get; set; } = false;

        [Export]
        public int Count { get; set; } = 1;

        [Export]
        public bool IsOneHanded { get; set; } = false;

        [Export]
        public Character CharacterOwner { get; set; } = null;

        [Export]
        public Texture2D Texture { get; set; }

        [Export]
        public string ScenePath { get; set; }

        /// <summary>
        /// Determines if this item can directly stack with other items
        /// </summary>
        public virtual bool StacksWith(ItemInfo itemInfo)
        {
            if (!CanStack)
            {
                return false;
            }

            if (ItemName != itemInfo.ItemName)
            {
                return false;
            }

            // several more conditions may be added soon

            return true;
        }

        public Item InstantiateItem(string name = "Primary")
        {
            var scene = ResourceLoader.Load<PackedScene>(ScenePath);
            var instance = scene.Instantiate<Item>();
            instance.Name = name;
            instance.Info = this;
            return instance;
        }
    }
}
