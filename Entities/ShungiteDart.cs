using Godot;
namespace SupaLidlGame.Entities;

public partial class ShungiteDart : Projectile
{
    public override void _Ready()
    {
        var player = GetNode<AnimationPlayer>("AnimationPlayer");
        player.Play("spin");
        player.SpeedScale = (float)(1 / Delay);
        base._Ready();
    }
}
