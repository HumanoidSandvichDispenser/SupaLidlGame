using Godot;
using SupaLidlGame.Characters;

namespace SupaLidlGame.Items;

public abstract partial class Item : Node2D
{
    [Export]
    public string ItemName { get; set; }

    [Export]
    public string Description { get; set; }

    [Export]
    public bool CanStack { get; set; } = false;

    public int Count { get; set; } = 1;

    public bool IsOneHanded { get; set; } = false;

    public Character CharacterOwner { get; set; }

    public virtual bool IsUsing => false;

    /// <summary>
    /// Determines if this item can directly stack with other items
    /// </summary>
    public virtual bool StacksWith(Item item)
    {
        if (!CanStack)
        {
            return false;
        }

        if (ItemName != item.ItemName)
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

    public virtual void UseAlt()
    {

    }

    public virtual void DeuseAlt()
    {

    }
}
