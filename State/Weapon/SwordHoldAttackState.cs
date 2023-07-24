using Godot;

namespace SupaLidlGame.State.Weapon;

/// <summary>
/// This is a special state only meant to be used with certain bosses.
/// </summary>
public partial class SwordHoldAttackState : SwordAttackState
{
    private bool _isAlternate = false;

    public override WeaponState Enter(IState<WeaponState> prevState)
    {
        Sword.EnableParry();
        Sword.Attack();

        if (HasAlternateAnimation && _isAlternate)
        {
            Sword.AnimationPlayer.Play("attack_alternate");
        }
        else
        {
            Sword.AnimationPlayer.Play("attack");
        }

        Sword.Visible = true;
        Sword.UseDirection = Sword.Character.Target;
        Sword.Hitbox.Hit += OnHitboxHit;
        return null;
    }

    private void OnHitboxHit(BoundingBoxes.BoundingBox box)
    {
        if (box is BoundingBoxes.Hurtbox hurtbox)
        {
            hurtbox.InflictDamage(Sword.Damage,
                Sword.Character,
                Sword.Knockback);
        }
        // damage player if not parried
        //Sword.OnHitboxHit(box);
    }

    public override void Exit(IState<WeaponState> nextState)
    {
        // reset hit & ignore list first because players are not immediately
        // removed from the list after being hit
        Sword.Hitbox.Hit -= OnHitboxHit;
        Sword.Hitbox.Hits.Clear();
        GD.Print("Cleared now with " + Sword.Hitbox.Hits.Count);
        Sword.Deattack();
    }

    public override WeaponState Use()
    {
        if (!Sword.IsAttacking)
        {
            GD.Print("Used (hold)");
            Sword.Attack();
        }

        return null;
    }

    public override WeaponState Deuse()
    {
        GD.Print("Deused");
        return IdleState;
    }

    public override WeaponState Process(double delta) => null;
}
