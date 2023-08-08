using Godot;
using SupaLidlGame.Items.Weapons;

namespace SupaLidlGame.State.Thinker;

public partial class BlockAttackState : AttackState
{
    public override void Attack(Characters.Character bestTarget)
    {
        if (NPC.Inventory.SelectedItem is Sword sword)
        {
            var stateMachine = sword.StateMachine;
            if (bestTarget.StunTime > 0)
            {
                NPC.UseCurrentItem();
            }
            else if (bestTarget.Inventory.SelectedItem is not Sword)
            {
                NPC.UseCurrentItem();
            }
            else
            {
                NPC.UseCurrentItemAlt();
            }
        }
    }
}
