using Godot;

namespace SupaLidlGame.State.Character;

public partial class PlayerHealState : PlayerState
{
    [Export]
    public double TimeToHeal { get; set; }

    [Export]
    public float HealAmount { get; set; }

    private double _timeLeftToHeal = 0;

    private bool _hasHealed = false;

    public override IState<CharacterState> Enter(IState<CharacterState> prevState)
    {
        if (_player.Stats.Level.Value < 1)
        {
            return IdleState;
        }
        _timeLeftToHeal = TimeToHeal;
        GD.Print("Heal anim");
        _player.MovementAnimation.Stop();
        _player.AttackAnimation.Play("heal_start");
        _player.AttackAnimation.Queue("heal");
        _hasHealed = false;
        return base.Enter(prevState);
    }

    public override void Exit(IState<CharacterState> nextState)
    {
        if (_hasHealed)
        {
            _player.AttackAnimation.Play("heal_end");
        }
        else
        {
            _player.AttackAnimation.Play("heal_cancel");
        }
        base.Exit(nextState);
    }

    public override CharacterState Process(double delta)
    {
        // lock player - no base.Process(delta)

        // if somehow the player's level decreased mid-heal and is not enough
        // then we return to idle
        if (_player.Stats.Level.Value < 1)
        {
            return IdleState;
        }

        if ((_timeLeftToHeal -= delta) <= 0)
        {
            // heal player
            _player.Stats.Level.Value--;
            _player.Health = Mathf.Min(_player.Health + HealAmount, 100);
            _hasHealed = true;
            return IdleState;
        }
        return null;
    }

    public override CharacterState Input(InputEvent @event)
    {
        if (@event.IsActionReleased("ability"))
        {
            _player.AttackAnimation.Play("heal_cancel");
            return IdleState;
        }
        return null;
    }
}
