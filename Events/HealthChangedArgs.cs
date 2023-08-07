namespace SupaLidlGame.Events;

public partial class HealthChangedArgs : Args
{
    public float OldHealth { get; set; }

    public float NewHealth { get; set; }

    public float Damage { get; set; }
}
