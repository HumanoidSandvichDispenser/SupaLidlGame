namespace SupaLidlGame.Events;

public partial class HealthChangedArgs : Args
{
    public Characters.Character Attacker { get; set; }

    public Items.Weapon Weapon { get; set; }

    public float OldHealth { get; set; }

    public float NewHealth { get; set; }

    public float Damage { get; set; }
}
