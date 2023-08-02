using Godot;

namespace SupaLidlGame.Events;

public partial class EventBus : Node
{
    [Signal]
    public delegate void RequestMoveToAreaEventHandler(RequestAreaArgs args);
}
