using System.Collections.Generic;
using Godot;
using SupaLidlGame.Characters;
using SupaLidlGame.Items;

namespace SupaLidlGame.BoundingBoxes;

public partial class Hitbox : BoundingBox
{
    private HashSet<BoundingBox> _ignoreList = new HashSet<BoundingBox>();

    [Signal]
    public delegate void HitEventHandler(BoundingBox box);

    [Export]
    public float Damage { get; set; }

    private bool _isDisabled = false;

    /// <summary>
    /// Getter/setter for the CollisionShape2D's Disabled property.
    /// </summary>
    [Export]
    public bool IsDisabled
    {
        get => _collisionShape.Disabled;
        set
        {
            _isDisabled = value;
            if (_collisionShape is not null)
            {
                _collisionShape.Disabled = value;
                if (value)
                {
                    DamageStartTime = Time.GetTicksMsec();
                }
            }
        }
    }

    [Export]
    public float Knockback { get; set; }

    public Character Inflictor { get; set; }

    public ulong DamageStartTime { get; set; }

    private CollisionShape2D _collisionShape;

    private bool _isParrying = false;

    public override void _Ready()
    {
        _collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        IsDisabled = _isDisabled; // sets _collisionShape.Disabled
    }

    private bool ShouldParry(Hitbox box)
    {
        Node parent = GetParent<Node>();

        // if this hitbox does not even belong to a weapon, skip
        if (parent is not Weapon)
        {
            return false;
        }

        var weapon = parent as Weapon;

        // if we hit a hitbox, we can parry if it can be parried
        if (box.GetParent<Node>() is Weapon other)
        {
            return weapon.IsParryable && other.IsParryable;
        }

        return false;
    }

    public void _on_area_entered(Area2D area)
    {
        if (area is BoundingBox box)
        {
            GD.Print("hit");
            // we don't want to hurt teammates
            if (Faction != box.Faction)
            {
                if (!_ignoreList.Contains(box))
                {
                    _ignoreList.Add(box);
                    EmitSignal(SignalName.Hit, box);
                }
            }
        }
    }

    public void ResetIgnoreList() => _ignoreList.Clear();

    public bool HasHit(BoundingBox box) => _ignoreList.Contains(box);

    public HashSet<BoundingBox> Hits
    {
        get => _ignoreList;
    }
}
