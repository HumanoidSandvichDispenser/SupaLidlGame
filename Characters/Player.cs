using Godot;
using GodotUtilities;
using SupaLidlGame.Utils;
using SupaLidlGame.BoundingBoxes;

namespace SupaLidlGame.Characters;

[Scene]
public sealed partial class Player : Character
{
    private AnimatedSprite2D _sprite;
    private string _spriteAnim;

    [Export]
    public PlayerCamera Camera { get; set; }

    [Export]
    public Marker2D DirectionMarker { get; private set; }

    public InteractionRay InteractionRay { get; private set; }

    public string Animation
    {
        get => _sprite.Animation;
        set
        {
            // the Player.Animation property might be set before this node is
            // even ready, so if _sprite is null, then we just hold the new
            // animation in a temp value, which will be assigned once this node
            // is ready
            if (_sprite is null)
            {
                _spriteAnim = value;
                return;
            }

            _sprite.Play(value);
        }
    }

    public override void _Ready()
    {
        InteractionRay = GetNode<InteractionRay>("Direction2D/InteractionRay");
        _sprite = GetNode<AnimatedSprite2D>("Sprite");
        if (_spriteAnim != default)
        {
            _sprite.Animation = _spriteAnim;
        }
        base._Ready();
    }

    public override void ModifyVelocity()
    {
        if (StateMachine.CurrentState is SupaLidlGame.State.Character.PlayerRollState)
        {
            Velocity *= 2;
        }

        base.ModifyVelocity();
    }

    public override void Stun(float time)
    {
        base.Stun(time);
        Camera.Shake(2, 0.8f);
        // TODO: implement visual effects for stun
    }

    public override void Die()
    {
        GD.Print("died");
        //base.Die();
    }

    protected override void DrawTarget()
    {
        base.DrawTarget();
        DirectionMarker.GlobalRotation = DirectionMarker.GlobalPosition
            .DirectionTo(GetGlobalMousePosition())
            .Angle();
    }
}
