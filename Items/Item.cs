using Godot;
using SupaLidlGame.Characters;

namespace SupaLidlGame.Items
{
    public abstract partial class Item : Node2D
    {
        [Export]
        public string Description { get; set; }

        public Character CharacterOwner { get; set; }

        public abstract void Equip(Character character);

        public abstract void Unequip(Character character);

        public abstract void Use();

        public abstract void Deuse();
    }
}
