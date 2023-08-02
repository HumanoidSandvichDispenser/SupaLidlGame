using Godot;

namespace SupaLidlGame.State.Character;

public partial class PlayerEmoteState : PlayerState
{
    public override IState<CharacterState> Enter(IState<CharacterState> prevState)
    {
        if (prevState != this)
        {
            _player.MovementAnimation.Play("emote");
        }

        if (!CanEmote(_player))
        {
            return IdleState;
        }

        return base.Enter(prevState);
    }

    public override CharacterState Process(double delta)
    {
        if (!CanEmote(_player))
        {
            return IdleState;
        }

        return base.Process(delta);
    }

    protected static bool CanEmote(Characters.Player player)
    {
        return !player.Inventory.IsUsingItem && player.Velocity.IsZeroApprox();
    }
}
