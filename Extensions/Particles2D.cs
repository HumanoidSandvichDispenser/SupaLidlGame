using Godot;

namespace SupaLidlGame.Extensions;

public static class Particles2D
{
    public static void EmitOneShot(this GpuParticles2D particles)
    {
        particles.GetTree().CreateTimer(particles.Lifetime).Timeout += () =>
        {
            particles.QueueFree();
        };
        particles.Emitting = true;
    }
}
