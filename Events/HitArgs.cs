namespace SupaLidlGame.Events;

public partial class HitArgs : HurtArgs
{
    public Characters.Character Victim { get; set; }
}
