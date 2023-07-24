using Godot;
using SupaLidlGame.Characters;

namespace SupaLidlGame.UI;

public partial class BossBar : VBoxContainer
{
    public TextureProgressBar ProgressBar { get; set; }
    public Label BossNameLabel { get; set; }

    private Boss _boss;

    public Boss Boss
    {
        get => _boss;
        set
        {
            SetupBoss(value);
            _boss = value;
        }
    }

    public override void _Ready()
    {
        ProgressBar = GetNode<TextureProgressBar>("BarMargin/BossBar");
        BossNameLabel = GetNode<Label>("LabelMargin/BossNameLabel");
    }

    private void OnBossHurt(Events.HealthChangedArgs args)
    {
        ProgressBar.Value = args.NewHealth;
    }

    private void OnBossDeath(Events.HealthChangedArgs args)
    {
        Visible = false;
        Boss = null;
    }

    private void SetupBoss(Boss newBoss)
    {
        if (_boss is not null)
        {
            _boss.Hurt -= OnBossHurt;
            _boss.Death -= OnBossDeath;
        }

        if (newBoss is not null)
        {
            newBoss.Hurt += OnBossHurt;
            newBoss.Death += OnBossDeath;

            ProgressBar.MaxValue = newBoss.Health;
            ProgressBar.Value = newBoss.Health;

            Visible = true;
        }
        else
        {
            Visible = false;
        }
    }
}
