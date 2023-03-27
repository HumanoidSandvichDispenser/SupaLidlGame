using Godot;
using System;

namespace SupaLidlGame.Characters
{
    public partial class Enemy : NPC
    {
        public override void _Ready()
        {
            //Inventory.SelectedItem = Inventory.GetNode<Items.Item>("Sword");
            Inventory.SelectFirstItem();
            base._Ready();
        }

        public override void Die()
        {
            base.Die();
        }
    }
}
