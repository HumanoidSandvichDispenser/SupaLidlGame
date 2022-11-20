using Godot;
using System;

namespace SupaLidlGame.Characters
{
    public partial class Enemy : NPC
    {
        public override void _Ready()
        {
            Inventory.SelectedItem = Inventory.GetNode<Items.Item>("Sword");
            base._Ready();
        }

        public override void Die()
        {
            base.Die();
        }
    }
}
