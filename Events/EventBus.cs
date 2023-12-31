using Godot;

namespace SupaLidlGame.Events;

public partial class EventBus : Node
{
    public static EventBus Instance { get; private set; }

    [Signal]
    public delegate void RequestMoveToAreaEventHandler(RequestAreaArgs args);

    [Signal]
    public delegate void AreaChangedEventHandler(Scenes.Map map);

    [Signal]
    public delegate void PlayerDeathEventHandler(HurtArgs args);

    [Signal]
    public delegate void PlayerHurtEventHandler(HurtArgs args);

    [Signal]
    public delegate void PlayerHitEventHandler(HitArgs args);

    [Signal]
    public delegate void PlayerXPChangedEventHandler(double xp);

    [Signal]
    public delegate void PlayerLevelChangedEventHandler(int level);

    [Signal]
    public delegate void PlayerInventoryUpdateEventHandler(Items.Inventory inventory);

    [Signal]
    public delegate void PlayerHealthChangedEventHandler(HealthChangedArgs args);

    [Signal]
    public delegate void RegisteredBossEventHandler(Characters.Boss boss);

    [Signal]
    public delegate void DeregisteredBossEventHandler(Characters.Boss boss);

    [Signal]
    public delegate void EnterTransitionEventHandler();

    [Signal]
    public delegate void TransitionFinishedEventHandler();

    [Signal]
    public delegate void ExitTransitionEventHandler();

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        if (Instance is not null)
        {
            throw new MultipleSingletonsException();
        }
        Instance = this;
    }
}
