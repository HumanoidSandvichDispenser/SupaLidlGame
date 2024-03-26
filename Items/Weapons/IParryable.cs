namespace SupaLidlGame.Items.Weapons;

public interface IParryable
{
    public bool IsParryable { get; }

    public bool HasParried { get; }

    public bool IsParried { get; }

    public ulong ParryTimeOrigin { get; }

    public void Stun();
}
