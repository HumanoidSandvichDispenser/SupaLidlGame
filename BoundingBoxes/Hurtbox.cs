using Godot;
using SupaLidlGame.Characters;

namespace SupaLidlGame.BoundingBoxes
{
    public partial class Hurtbox : Area2D
    {
        [Signal]
        public delegate void ReceivedDamageEventHandler(float damage);

        public void InflictDamage(
            float damage,
            Character inflictor,
            float knockback,
            Vector2 knockbackOrigin = default,
            Vector2 knockbackVector = default)
        {
            EmitSignal(
                "ReceivedDamage",
                damage,
                inflictor,
                knockback,
                knockbackOrigin, knockbackVector);
        }
    }
}
