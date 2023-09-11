using Godot;

namespace SupaLidlGame.State.Character;

public partial class PlayerIdleState : PlayerState
{
    [Export]
    public CharacterState MoveState { get; set; }

    [Export]
    public PlayerEmoteState EmoteState { get; set; }

    [Export]
    public PlayerHealState HealState { get; set; }

    public override IState<CharacterState> Enter(IState<CharacterState> previousState)
    {
        if (previousState is not PlayerMoveState)
        {
            if (Character.Direction.LengthSquared() > 0.01f)
            {
                // other states like attacking or rolling can just delegate
                // the work of checking if the player is moving to this idle
                // state, so they do not have to manually check if the player
                // wants to move to switch to the move state.
                return MoveState;
            }
        }

        // must be moving at least 4 u/s for more than 0.5 seconds
        bool shouldPlayStopAnim = false;

        if (previousState is PlayerMoveState move)
        {
            shouldPlayStopAnim = move.MoveDuration > 0.5;
            // NOTE: more conditions may be added soon
        }

        if (Godot.Input.IsActionPressed("ability"))
        {
            if (CanHeal())
            {
                return HealState;
            }
        }

        if (shouldPlayStopAnim)
        {
            _player.MovementAnimation.Play("stop");
            _player.MovementAnimation.Queue("idle");
        }
        else
        {
            _player.MovementAnimation.Play("idle");
        }

        return base.Enter(previousState);
    }

    public override CharacterState Input(InputEvent @event)
    {
        if (@event.IsActionPressed("emote"))
        {
            return EmoteState;
        }

        return base.Input(@event);
    }

    public override CharacterState Process(double delta)
    {
        base.Process(delta);
        if (Character.Direction.LengthSquared() > 0)
        {
            return MoveState;
        }

        if (Godot.Input.IsActionPressed("ability"))
        {
            if (CanHeal())
            {
                return HealState;
            }
        }

        return null;
    }

    private bool CanHeal()
    {
        return !_player.Inventory.IsUsingItem && _player.Stats.Level.Value > 0;
    }
}
