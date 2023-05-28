using Godot;
using Godot.Collections;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.Utils
{
    public partial class Spawner : Node2D
    {
        [Export]
        public Area2D SpawnArea { get; set; }

        [Export]
        public PackedScene Character { get; set; }

        [Export]
        public double SpawnTime { get; set; }

        protected double _timeLeftToSpawn = 0;

        public override void _Ready()
        {
        }

        public override void _Process(double delta)
        {
            if ((_timeLeftToSpawn -= delta) <= 0)
            {
                _timeLeftToSpawn = SpawnTime;
                Spawn();
            }
        }

        public void Spawn()
        {
            var coll = GetNode<CollisionShape2D>("Area2D/CollisionShape2D");
            var rect = coll.Shape as RectangleShape2D;
            if (rect is not null)
            {
                Vector2 size = rect.Size / 2;
                Vector2 pos = GlobalPosition;

                double x1, x2, y1, y2;
                x1 = pos.X - size.X;
                x2 = pos.X + size.X;
                y1 = pos.Y - size.Y;
                y2 = pos.Y + size.Y;

                float randX = (float)GD.RandRange(x1, x2);
                float randY = (float)GD.RandRange(y1, y2);

                Vector2 randPos = new Vector2(randX, randY);

                var chr = Character.Instantiate<Characters.Character>();
                chr.GlobalPosition = randPos;
                this.GetAncestor<Scenes.Map>().Entities.AddChild(chr);
            }
        }
    }
}
