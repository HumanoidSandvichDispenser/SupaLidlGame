using Godot;

namespace SupaLidlGame.Events;

public partial class EventBus : Node
{
    [Signal]
    public delegate void RequestMoveToAreaEventHandler(RequestAreaArgs args);

    [Signal]
    public delegate void AreaChangedEventHandler(Scenes.Map map);

    [Signal]
    public delegate void PlayerDeathEventHandler(HurtArgs args);

    [Signal]
    public delegate void PlayerHurtEventHandler(HurtArgs args);

    [Signal]
    public delegate void PlayerHealthChangedEventHandler(HealthChangedArgs args);

    [Signal]
    public delegate void RegisteredBossEventHandler(Characters.Boss boss);

    [Signal]
    public delegate void DeregisteredBossEventHandler(Characters.Boss boss);
}
