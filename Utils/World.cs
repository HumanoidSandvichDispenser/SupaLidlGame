using Godot;
using SupaLidlGame.Characters;
using System.Collections.Generic;

namespace SupaLidlGame.Utils
{
    public partial class World : Node2D
    {
        [Export]
        public string StartingArea { get; set; }

        [Export]
        public string CurrentArea { get; protected set; }

        [Export]
        public Player CurrentPlayer { get; set; }

        private Dictionary<string, TileMap> maps;

        private string _currentConnector;

        public World()
        {
            maps = new Dictionary<string, TileMap>();
        }

        public override void _Ready()
        {
            base._Ready();
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
        }

        public void MoveToArea(string area, string connector, Player player)
        {
            if (area != CurrentArea)
            {
                // remove current map and load in the new map
                TileMap map = GetNode<TileMap>(CurrentArea);
                CurrentArea = area;
            }

            _currentConnector = connector;
        }

        public void _on_area_2d_requested_enter(
            BoundingBoxes.ConnectorBox box,
            Player player)
        {
            MoveToArea(box.ToArea, box.ToConnector, player);
        }
    }
}
