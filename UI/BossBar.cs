using Godot;
using SupaLidlGame.Characters;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.UI;

public partial class BossBar : VBoxContainer
{
    public TextureProgressBar ProgressBar { get; set; }

    public Label BossNameLabel { get; set; }

    private Events.EventBus _eventBus;
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
        _eventBus = this.GetEventBus();
        _eventBus.RegisteredBoss += RegisterBoss;
        _eventBus.DeregisteredBoss += DeregisterBoss;
    }

    private void RegisterBoss(Boss boss)
    {
        Boss = boss;
    }

    private void DeregisterBoss(Boss boss)
    {
        Boss = null;
    }

    private void OnBossHurt(Events.HurtArgs args)
    {
        ProgressBar.Value = args.NewHealth;
    }

    private void OnBossDeath(Events.HurtArgs args)
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
