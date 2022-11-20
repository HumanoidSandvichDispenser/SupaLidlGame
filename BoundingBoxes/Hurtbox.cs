using Godot;
using SupaLidlGame.Characters;
using SupaLidlGame.Utils;

namespace SupaLidlGame.BoundingBoxes
{
    public partial class Hurtbox : Area2D, IFaction
    {
        [Signal]
        public delegate void ReceivedDamageEventHandler(
            float damage,
            Character inflictor,
            float knockback,
            Vector2 knockbackOrigin = default,
            Vector2 knockbackVector = default);

        [Export]
        public ushort Faction { get; set; }

        public override void _Ready()
        {
            if (Faction == default && GetParent() is IFaction factionEntity)
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
            EmitSignal(
                SignalName.ReceivedDamage,
                damage,
                inflictor,
                knockback,
                knockbackOrigin, knockbackVector);
        }
    }
}
