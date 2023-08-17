using Godot;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.UI;

public partial class SceneTransition : Control
{
    private AnimationPlayer _animPlayer;
    private Events.EventBus _bus;

    public override void _Ready()
    {
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _bus = this.GetEventBus();
        _bus.EnterTransition += Enter;
        _bus.ExitTransition += Exit;
    }

    public async void Enter()
    {
        _animPlayer.Play("enter");
        await ToSignal(_animPlayer, "animation_finished");
        _bus.EmitSignal(Events.EventBus.SignalName.TransitionFinished);
    }

    public void Exit()
    {
        _animPlayer.Play("exit");
    }
}
