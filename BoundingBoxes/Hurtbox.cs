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
        Vector2 knockbackDir = default);

    /// <summary>
    /// The timer to use for invincibility frames
    /// </summary>
    [Export]
    public Timer InvincibilityTimer { get; set; }

    public override void _Ready()
    {
        if (GetParent() is IFaction factionEntity)
        {
            Faction = factionEntity.Faction;
        }

        if (InvincibilityTimer is not null)
        {
            InvincibilityTimer.Timeout += () =>
            {
                GD.Print("invincibility off");
                Monitorable = true;
            };
        }
    }

    public void InflictDamage(
        float damage,
        Character inflictor,
        float knockback,
        Vector2 knockbackOrigin = default,
        Vector2 knockbackVector = default)
    {
        if (!IsInstanceValid(this))
        {
            // this should fix the error of the object being invalid
            return;
        }

        Vector2 knockbackDir = knockbackVector;
        if (knockbackDir == default)
        {
            if (knockbackOrigin == default)
            {
                if (inflictor is null)
                {
                    knockbackOrigin = GlobalPosition + Vector2.Down;
                }
                else
                {
                    knockbackOrigin = inflictor.GlobalPosition;
                }
            }

            knockbackDir = knockbackOrigin.DirectionTo(GlobalPosition);
        }

        if (InvincibilityTimer is not null)
        {
            if (!InvincibilityTimer.IsStopped())
            {
                return;
            }

            InvincibilityTimer.Start();
            //Monitorable = false;
            SetDeferred("monitorable", false);
            GD.Print("invincible");
        }

        EmitSignal(
            SignalName.ReceivedDamage,
            damage,
            inflictor,
            knockback,
            knockbackDir);
    }
}
