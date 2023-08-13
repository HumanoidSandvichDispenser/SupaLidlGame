using Godot;

namespace SupaLidlGame.Events;

public partial class EventBus : Node
{
    [Signal]
    public delegate void RequestMoveToAreaEventHandler(RequestAreaArgs args);

    [Signal]
    public delegate void PlayerDeathEventHandler(HurtArgs args);

    [Signal]
    public delegate void PlayerHurtEventHandler(HurtArgs args);

    [Signal]
    public delegate void PlayerHealthChangedEventHandler(HealthChangedArgs args);
}
