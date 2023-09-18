using Godot;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.Entities;

public partial class Lightning : Projectile
{
    private AnimationPlayer _animPlayer;

    public override void _Ready()
    {
        base._Ready();
        _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        _animPlayer.Play("start");
        _animPlayer.AnimationFinished += (anim) => Die();

        var stream = GetNode<Node2D>("AudioStreamPlayer2D");
        stream.CloneOnWorld<AudioStreamPlayer2D>()
            .WithPitchDeviation(0.125f)
            .Play();

        // hitbox should only be active for one frame only
        //Hitbox.SetDeferred("monitoring", false);

        Hitbox.Hit += (box) =>
        {
            if (box is BoundingBoxes.Hurtbox)
            {
                if (box.GetParent() is Characters.Character ch)
                {
                    ch.Stun(3);
                }
            }
        };
    }
}
