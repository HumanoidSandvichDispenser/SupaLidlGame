using Godot;

namespace SupaLidlGame.Items.Weapons
{
    public abstract partial class Ranged : Weapon
    {
        [Export]
        public float AngleDeviation { get; set; }

        [Export]
        public float ChargeTime { get; set; }

        public bool IsChargeable => ChargeTime > 0;

        public double ChargeProgress { get; protected set; }

        public bool IsCharging { get; protected set; }

        public override void Use()
        {
            if (RemainingUseTime > 0)
            {
                return;
            }

            if (IsChargeable)
            {
                IsCharging = true;
            }
            else
            {
                Attack();
            }

            base.Use();
        }

        public override void Deuse()
        {
            if (IsChargeable && IsCharging)
            {
                Attack();
                IsCharging = false;
            }

            base.Deuse();
        }

        public override void _Process(double delta)
        {
            if (IsCharging)
            {
                ChargeProgress += delta;
            }

            base._Process(delta);
        }

        public abstract void Attack();
    }
}
