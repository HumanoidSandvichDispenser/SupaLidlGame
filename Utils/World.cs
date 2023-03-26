using Godot;
using SupaLidlGame.Characters;
using SupaLidlGame.Scenes;
using SupaLidlGame.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace SupaLidlGame.Utils
{
    public partial class World : Node2D
    {
        [Export]
        public PackedScene StartingArea { get; set; }

        [Export]
        public Map CurrentMap { get; protected set; }

        [Export]
        public Player CurrentPlayer { get; set; }

        private Dictionary<string, Map> _maps;

        private string _currentConnector;

        private string _currentMapResourcePath;

        private const string PLAYER_PATH = "res://Characters/Player.tscn";
        private PackedScene _playerScene;

        public World()
        {
            _maps = new Dictionary<string, Map>();
            _playerScene = ResourceLoader.Load<PackedScene>(PLAYER_PATH);
        }

        public override void _Ready()
        {
            if (StartingArea is not null)
            {
                LoadScene(StartingArea);
            }

            // spawn the player in
            CreatePlayer();

            base._Ready();
        }

        public void LoadScene(PackedScene scene)
        {
            GD.Print("Loading map " + scene.ResourcePath);

            Map map;
            if (_maps.ContainsKey(scene.ResourcePath))
            {
                map = _maps[scene.ResourcePath];
            }
            else
            {
                map = scene.Instantiate<Map>();
                _maps.Add(scene.ResourcePath, map);
            }

            if (CurrentMap is not null)
            {
                CurrentMap.Entities.RemoveChild(CurrentPlayer);
                RemoveChild(CurrentMap);
                CurrentMap.Active = false;
            }

            AddChild(map);
            InitTilemap(map);

            CurrentMap = map;
            CurrentMap.Active = true;

            if (CurrentPlayer is not null)
            {
                CurrentMap.Entities.AddChild(CurrentPlayer);
            }
        }

        public void CreatePlayer()
        {
            CurrentPlayer = _playerScene.Instantiate<Player>();
            CurrentMap.Entities.AddChild(CurrentPlayer);
        }

        private void InitTilemap(Map map)
        {
            var children = map.Areas.GetChildren();
            foreach (Node node in children)
            {
                if (node is BoundingBoxes.ConnectorBox connector)
                {
                    // this reconnects the EventHandler if it is connected
                    connector.RequestedEnter -= _on_area_2d_requested_enter;
                    connector.RequestedEnter += _on_area_2d_requested_enter;
                }
            }
        }

        private void MovePlayerToConnector(string name)
        {
            // find the first connector with the specified name
            var connector = CurrentMap.Areas.GetChildren().First((child) =>
            {
                if (child is BoundingBoxes.ConnectorBox connector)
                {
                    return connector.Identifier == name;
                }
                return false;
            }) as BoundingBoxes.ConnectorBox;
            
            CurrentPlayer.GlobalPosition = connector.GlobalPosition;
        }

        public void MoveToArea(string path, string connector)
        {
            _currentConnector = connector;
            if (path != _currentMapResourcePath)
            {
                var scene = ResourceLoader.Load<PackedScene>(path);
                LoadScene(scene);
                _currentMapResourcePath = path;
            }

            // after finished loading, move our player to the connector
            MovePlayerToConnector(connector);
        }

        public void _on_area_2d_requested_enter(
            BoundingBoxes.ConnectorBox box,
            Player player)
        {
            GD.Print("Requesting to enter " + box.ToConnector);
            MoveToArea(box.ToArea, box.ToConnector);
        }

        public void SaveGame()
        {
            throw new System.NotImplementedException();
        }

        public void LoadGame()
        {
            throw new System.NotImplementedException();
        }
    }
}
