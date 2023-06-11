using Godot;
using GodotUtilities;
using SupaLidlGame.BoundingBoxes;

namespace SupaLidlGame.Entities;

[Scene]
public partial class Campfire : StaticBody2D, Utils.IInteractive
{
    private PointLight2D _light;

    public InteractionTrigger InteractionTrigger { get; set; }

    [Signal]
    public delegate void UseEventHandler();

    public override void _Ready()
    {
        InteractionTrigger = GetNode<InteractionTrigger>("InteractionTrigger");
        _light = GetNode<PointLight2D>("PointLight2D");
        InteractionTrigger.Interaction += () =>
        {
            // save the player's spawn position to be their position on interaction
            EmitSignal(SignalName.Use);

            this.GetAncestor<Utils.World>().SetSpawn(GlobalPosition);
        };
    }

    public override void _Process(double delta)
    {
        _light.Energy += (GD.Randf() - 0.5f) * 8 * (float)delta;
        _light.Energy = Mathf.Clamp(_light.Energy, 1.8f, 2f);
    }
}
