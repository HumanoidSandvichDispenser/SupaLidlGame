using Godot;
using GodotUtilities;
using SupaLidlGame.BoundingBoxes;
using SupaLidlGame.Extensions;
using GodotUtilities.SourceGenerators;
using DialogueManagerRuntime;

namespace SupaLidlGame.Entities;

[Scene]
public partial class Campfire : StaticBody2D, Utils.IInteractive
{
    [Node("PointLight2D")]
    private PointLight2D _light;

    [Node("InteractionTrigger")]
    public InteractionTrigger InteractionTrigger { get; set; }

    [Signal]
    public delegate void UseEventHandler();

    [Export(PropertyHint.File, "*.dialogue")]
    public Resource DialogueResource { get; set; }

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            WireNodes();
        }
    }

    public override void _Ready()
    {
        InteractionTrigger.Interaction += () =>
        {
            // save the player's spawn position to be their position on interaction
            EmitSignal(SignalName.Use);

            var world = this.GetWorld();

            // TODO: implement and use max health
            world.CurrentPlayer.Health = 100;

            //this.GetAncestor<Utils.World>().SetSpawn(GlobalPosition);
            world.DialogueBalloon.Start(DialogueResource, "start");
        };
    }

    public override void _Process(double delta)
    {
        _light.Energy += (GD.Randf() - 0.5f) * 8 * (float)delta;
        _light.Energy = Mathf.Clamp(_light.Energy, 1.8f, 2f);
    }
}
