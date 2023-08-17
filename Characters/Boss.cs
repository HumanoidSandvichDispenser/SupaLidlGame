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

    [Export]
    public Vector2 InitialPosition { get; set; }

    public abstract int Intensity { get; }

    private bool _isActive;
    private Events.EventBus _eventBus;

    [Export]
    public virtual bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;

            // register or deregister ourselves when we are active/inactive
            _eventBus.EmitSignal(
                Events.EventBus.SignalName.RegisteredBoss,
                _isActive ? this : null
            );
        }
    }

    [Export]
    public float MaxHealth { get; set; }

    public override void _Ready()
    {
        base._Ready();

        Death += (Events.HurtArgs args) =>
        {
            UpdateBossStatus(true);
        };

        this.GetWorld().CurrentPlayer.Death += (args) =>
        {
            IsActive = false;
        };

        _eventBus = this.GetEventBus();
    }

    protected void UpdateBossStatus(bool status)
    {
        GetNode<State.Global.GlobalState>("/root/GlobalState")
            .Progression.BossStatus[SceneFilePath] = status;
    }

    protected virtual void Reset()
    {
        // reset animations
        foreach (var child in GetNode("Animations").GetChildren())
        {
            if (child is AnimationPlayer anim)
            {
                if (anim.HasAnimation("RESET"))
                {
                    anim.Play("RESET");
                }
            }
        }

        StateMachine.ChangeState(StateMachine.InitialState);
        ThinkerStateMachine.ChangeState(ThinkerStateMachine.InitialState);
        Health = MaxHealth;
    }
}
