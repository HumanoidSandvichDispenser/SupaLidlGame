using Godot;

namespace SupaLidlGame.State.Weapon;

public partial class SwordAnticipateState : WeaponState
{
    [Export]
    public SupaLidlGame.Items.Weapons.Sword Sword { get; set; }
    
    [Export]
    public SwordAttackState AttackState { get; set; }

    [Export]
    public bool HasAlternateAninmation { get; set; } = false;

    private double _anticipateTime;

    public override WeaponState Enter(IState<WeaponState> prevState)
    {
        if (Sword.Character is SupaLidlGame.Characters.Player)
        {
            return AttackState;
        }

        float rotThreshold = Mathf.DegToRad(50);
        if (HasAlternateAninmation && Sword.Anchor.Rotation > rotThreshold)
        {
            Sword.AnimationPlayer.Play("anticipate_alternate");
        }
        else
        {
            Sword.AnimationPlayer.Play("anticipate");
        }
        _anticipateTime = Sword.NPCAnticipateTime;
        return null;
    }

    public override WeaponState Process(double delta)
    {
        // go into attack state if anticipation time is delta
        if ((_anticipateTime -= delta) <= 0)
        {
            return AttackState;
        }
        return null;
    }
}
