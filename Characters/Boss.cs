using Godot;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.Characters;

public abstract partial class Boss : Enemy
{
    [Export]
    public State.NPC.NPCStateMachine BossStateMachine { get; set; }

    [Export]
    public string BossName { get; set; }

    [Export]
    public AudioStream Music { get; set; }

    public abstract int Intensity { get; }

    private bool _isActive;

    [Export]
    public virtual bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;

            // register or deregister ourselves when we are active/inactive
            this.GetWorld().RegisterBoss(_isActive ? this : null);
        }
    }

    public override void _Ready()
    {
        base._Ready();

        Death += (Events.HealthChangedArgs args) =>
        {
            UpdateBossStatus(true);
        };
    }

    protected void UpdateBossStatus(bool status)
    {
        GetNode<State.Global.GlobalState>("/root/GlobalState")
            .Progression.BossStatus[SceneFilePath] = status;
    }
}
