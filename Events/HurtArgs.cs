namespace SupaLidlGame.Events;

public partial class HurtArgs : HealthChangedArgs
{
    public Characters.Character Attacker { get; set; }

    public Items.Weapon Weapon { get; set; }
}
