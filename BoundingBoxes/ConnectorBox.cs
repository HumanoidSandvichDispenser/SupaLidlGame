using Godot;
using System;

namespace SupaLidlGame.BoundingBoxes
{
    public partial class ConnectorBox : Area2D
    {
        [Export]
        public string ToArea { get; set; }

        [Export]
        public string ToConnector { get; set; }
    }
}
