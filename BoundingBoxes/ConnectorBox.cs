using Godot;
using System;
using SupaLidlGame.Characters;
using SupaLidlGame.Extensions;
using SupaLidlGame.Events;

namespace SupaLidlGame.BoundingBoxes;

public partial class ConnectorBox : Area2D
{
    [Signal]
    public delegate void RequestedEnterEventHandler(
        ConnectorBox box,
        Player player);

    [Export(PropertyHint.File, "*.tscn")]
    public string ToArea { get; set; }

    [Export]
    public string ToConnector { get; set; }

    [Export]
    public string Identifier { get; set; }

    /// <summary>
    /// Determines if the connector requires the user to interact to enter
    /// the connector
    /// </summary>
    [Export]
    public InteractionTrigger InteractionTrigger { get; set; }

    private Player _player = null;

    public override void _Ready()
    {
        BodyEntered += (Node2D body) =>
        {
            if (body is Player && InteractionTrigger is null)
            {
                OnInteraction();
            }
        };

        if (InteractionTrigger is not null)
        {
            InteractionTrigger.Interaction += OnInteraction;
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    protected void OnInteraction()
    {
        var eventBus = this.GetEventBus();
        System.Diagnostics.Debug.Assert(eventBus is not null);
        var args = new Events.RequestAreaArgs
        {
            Area = ToArea,
            Connector = ToConnector,
        };
        eventBus.EmitSignal(EventBus.SignalName.RequestMoveToArea, args);
    }
}
