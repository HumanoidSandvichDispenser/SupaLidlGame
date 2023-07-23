using Godot;
using SupaLidlGame.BoundingBoxes;
using SupaLidlGame.Characters;

namespace SupaLidlGame.Items;

public abstract partial class Weapon : Item
{
    public double RemainingUseTime { get; protected set; } = 0;

    public override bool IsUsing => RemainingUseTime > 0;

    /// <summary>
    /// How much damage in HP that this weapon deals.
    /// </summary>
    [Export]
    public float Damage { get; set; } = 0;

    /// <summary>
    /// The time in seconds it takes for this weapon to become available
    /// again after using.
    /// </summary>
    [Export]
    public double UseTime { get; set; } = 0;

    /// <summary>
    /// The magnitude of the knockback force of the weapon.
    /// </summary>
    [Export]
    public float Knockback { get; set; } = 0;

    /// <summary>
    /// The initial velocity of any projectile the weapon may spawn.
    /// </summary>
    [Export]
    public float InitialVelocity { get; set; } = 0;

    /// <summary>
    /// Hides the weapon if it is idle (i.e. not being used).
    /// </summary>
    [Export]
    public bool ShouldHideIdle { get; set; } = false;

    /// <summary>
    /// Freezes the player's target angle if this weapon is being used.
    /// </summary>
    [Export]
    public bool ShouldFreezeAngleOnUse { get; set; } = true;

    [Export]
    public float MinDistanceHint { get; set; }

    [Export]
    public float MaxDistanceHint { get; set; }

    [Export]
    public Sprite2D HandAnchor { get; set; }

    public virtual bool IsParryable { get; protected set; } = false;

    public bool IsParried { get; set; }

    public Character Character { get; set; }

    public Vector2 UseDirection { get; set; }

    public override bool StacksWith(Item item) => false;

    public override void Equip(Character character)
    {
        if (!ShouldHideIdle || IsUsing)
        {
            Visible = true;
        }
        Character = character;

        // set the hand textures to the character's
        if (HandAnchor is not null && character.HandTexture is not null)
        {
            HandAnchor.Texture = character.HandTexture;
        }
    }

    public override void Unequip(Character character)
    {
        Visible = false;
        Character = null;
    }

    public override void Use()
    {
        RemainingUseTime = UseTime;
    }

    public override void Deuse()
    {

    }

    public override void _Process(double delta)
    {
        if (RemainingUseTime > 0)
        {
            if ((RemainingUseTime -= delta) <= 0)
            {
                //Deuse();
            }
        }
    }

    public virtual void _on_hitbox_hit(BoundingBox box)
    {
        if (box is Hurtbox hurtbox)
        {
            hurtbox.InflictDamage(Damage, Character, Knockback);
        }
    }
}
