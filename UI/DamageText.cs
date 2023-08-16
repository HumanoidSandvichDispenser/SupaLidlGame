using Godot;

namespace SupaLidlGame.UI;

public partial class DamageText : FloatingText
{
    private float _damage = 0;

    public float Damage
    {
        get => _damage;
        set
        {
            _damage = value;
            Text = Mathf.Round(value).ToString();
        }
    }
}
