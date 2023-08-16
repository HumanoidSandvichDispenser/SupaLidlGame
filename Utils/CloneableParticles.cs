using Godot;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.Utils;

public partial class CloneableParticles : GpuParticles2D
{
    public void EmitOnWorld()
    {
        var world = this.GetWorld();
        var entities = world.CurrentMap.Entities;
        var clone = Duplicate() as GpuParticles2D;
        clone.GlobalTransform = GlobalTransform;
        entities.AddChild(clone);
        clone.Emitting = true;
        GetTree().CreateTimer(Lifetime).Timeout += () =>
        {
            clone?.QueueFree();
        };
    }
}
