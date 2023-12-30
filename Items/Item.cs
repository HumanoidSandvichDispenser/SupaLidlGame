using Godot;
using SupaLidlGame.Characters;

namespace SupaLidlGame.Items;

public abstract partial class Item : Node2D
{
    [Signal]
    public delegate void UsedItemEventHandler(Item item);

    [Export]
    public bool CanStack { get; set; } = false;

    [Export]
    public ItemMetadata Metadata { get; set; }

    public int Count { get; set; } = 1;

    public bool IsOneHanded { get; set; } = false;

    public Character CharacterOwner { get; set; }

    /// <summary>
    /// Determines if the item is being used. This property determines if
    /// a character can use another item or not.
    /// See <see cref="Character.UseCurrentItem"/>
    /// </summary>
    /// 
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

        if (Metadata.Name != item.Metadata.Name)
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
