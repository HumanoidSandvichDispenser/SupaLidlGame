using Godot;
using SupaLidlGame.Characters;
using System;

public partial class OscillatingBody : CharacterBody2D
{
    private double _time = 0;

    public override void _PhysicsProcess(double delta)
    {
        _time += delta;
        // move along the path sin(x) whose derivative is cos(x)
        Velocity = Vector2.Left * Mathf.Cos((float)_time) * 256;
        MoveAndSlide();
    }

    public void _on_hurtbox_received_damage(float damage, Character attacker,
                                            float knockback, Vector2 _, Vector2 __)
    {
        GD.Print($"took {damage} dmg");
    }
}
