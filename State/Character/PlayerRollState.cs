using Godot;

namespace SupaLidlGame.State.Character;

public partial class PlayerRollState : PlayerState
{
    private double _timeLeftToRoll = 0;

    private Vector2 _rollDirection = Vector2.Zero;

    private GpuParticles2D _particles;

    public float VelocityModifier { get; set; } = 1.5f;

    public PlayerRollState()
    {
        CanEnterWhileUsingItem = false;
    }

    public override void _Ready()
    {
        base._Ready();
        _particles = _player.GetNode<GpuParticles2D>("Effects/RollParticles");
    }

    public override IState<CharacterState> Enter(IState<CharacterState> previousState)
    {
        _timeLeftToRoll = 0.5;
        // roll the direction we were previously moving in
        _rollDirection = Character.Direction;
        Character.Target = Character.Direction;
        if (Character.Direction.X >= 0)
        {
            _player.MovementAnimation.Play("roll");
        }
        else
        {
            _player.MovementAnimation.PlayBackwards("roll");
        }
        _player.MovementAnimation.Queue("idle");
        _particles.Emitting = true;
        return base.Enter(previousState);
    }

    public override void Exit(IState<CharacterState> nextState)
    {
        // we want to reset our state variables in case we are forced out of
        // this state (e.g. from death)
        _timeLeftToRoll = 0;
        _rollDirection = Character.Direction;
        _particles.Emitting = false;
        if (_player.MovementAnimation.CurrentAnimation == "roll")
        {
            // prevents player's sprite from being at an angle when exiting too
            // soon
            _player.MovementAnimation.Stop();
        }
        base.Exit(nextState);
    }

    public override CharacterState Process(double delta)
    {
        Character.Direction = _rollDirection;
        if ((_timeLeftToRoll -= delta) <= 0 || _player.Health <= 0)
        {
            return IdleState;
        }
        return null;
    }
}
