using Godot;
using SupaLidlGame.Characters;
using SupaLidlGame.Utils;

namespace SupaLidlGame.BoundingBoxes;

public partial class Hurtbox : BoundingBox, IFaction
{
    [Signal]
    public delegate void ReceivedDamageEventHandler(
        float damage,
        Character inflictor,
        float knockback,
        Vector2 knockbackOrigin = default,
        Vector2 knockbackVector = default);

    public override void _Ready()
    {
        if (GetParent() is IFaction factionEntity)
        {
            Faction = factionEntity.Faction;
        }
    }

    public void InflictDamage(
        float damage,
        Character inflictor,
        float knockback,
        Vector2 knockbackOrigin = default,
        Vector2 knockbackVector = default)
    {
        if (inflictor is not null)
        {
            EmitSignal(
                SignalName.ReceivedDamage,
                damage,
                inflictor,
                knockback,
                knockbackOrigin, knockbackVector);
        }
    }
}
