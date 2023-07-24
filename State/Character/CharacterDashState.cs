using Godot;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.State.Character;

public partial class CharacterDashState : CharacterState
{
    [Export]
    public CharacterState IdleState { get; set; }

    [Export]
    public double TimeToDash { get; set; }

    [Export]
    public float VelocityModifier { get; set; }

    private double _timeLeftToDash = 0;

    public Vector2 DashDirection = Vector2.Zero;

    public override IState<CharacterState> Enter(IState<CharacterState> previousState)
    {
        _timeLeftToDash = TimeToDash;
        // dash the direction we were previously moving in
        DashDirection = Character.Direction;
        Character.MovementAnimation.Play("dash");
        Character.MovementAnimation.Queue("idle");

        // create ghost effect
        var ghost = Character.Sprite.CloneOnWorld<Sprite2D>();
        ghost.GlobalPosition = Character.Sprite.GlobalPosition;
        var tween = ghost.GetTree().CreateTween();
        tween.TweenProperty(ghost, "self_modulate", Colors.Transparent, 0.5);
        tween.TweenCallback(new Callable(ghost, "queue_free"));
        tween.Play();

        return base.Enter(previousState);
    }

    public override void Exit(IState<CharacterState> nextState)
    {
        _timeLeftToDash = 0;
        DashDirection = Character.Direction;
        base.Exit(nextState);
    }

    public override CharacterState Process(double delta)
    {
        Character.Direction = DashDirection;
        if ((_timeLeftToDash -= delta) <= 0 || Character.Health <= 0)
        {
            return IdleState;
        }
        return null;
    }
}
