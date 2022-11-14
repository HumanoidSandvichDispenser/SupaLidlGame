using Godot;
using System;

namespace SupaLidlGame.Prototyping
{
    public partial class ContextBasedSteering : Node2D
    {
        float[] _weights = new float[16];
        Vector2[] _weightDirs = new Vector2[16];
        int _bestWeightIdx;

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
            Array.Fill(_weights, 1);
            for (int i = 0; i < 16; i++)
            {
                float y = Mathf.Sin(Mathf.Pi * i * 2 / 16);
                float x = Mathf.Cos(Mathf.Pi * i * 2 / 16);
                _weightDirs[i] = new Vector2(x, y);
            }
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
            Vector2 pos = GetLocalMousePosition();
            Vector2 dir = GlobalPosition.DirectionTo(pos);
            float dist = GlobalPosition.DistanceSquaredTo(pos);

            for (int i = 0; i < 16; i++)
            {
                float directDot = _weightDirs[i].Dot(dir);
                directDot = (directDot + 1) / 2;

                // this dot product resembles values of sine rather than cosine
                // use it to weigh direction horizontally
                Vector2 rotatedDir = new Vector2(-dir.y, dir.x);
                float horizDot = Math.Abs(_weightDirs[i].Dot(rotatedDir));
                // square so lower values are even lower
                horizDot = Mathf.Pow((horizDot + 1) / 2, 2);

                // "When will I use math in the real world" Clueful


                if (dist > 1024)
                {
                    _weights[i] = directDot;
                }
                else if (dist > 256)
                {
                    float directDotWeighting = (dist - 256) / 768;
                    float horizDotWeighting = 1 - directDotWeighting;

                    _weights[i] = (directDot * directDotWeighting) +
                        (horizDot * horizDotWeighting);
                }
                else
                {
                    // shorter than 256
                    _weights[i] = horizDot;
                }

                // now we shall subtract weights whose rays collide
                // with something

                {

                }
            }

            float bestWeight = 0;
            for (int i = 0; i < 16; i++)
            {
                if (_weights[i] > bestWeight)
                {
                    bestWeight = _weights[i];
                    _bestWeightIdx = i;
                }
            }

            QueueRedraw();
        }

        public override void _Draw()
        {
            for (int i = 0; i < 16; i++)
            {
                Vector2 vec = _weightDirs[i] * _weights[i] * 128;
                Color c = Colors.Green;
                if (_bestWeightIdx == i)
                {
                    c = Colors.Blue;
                }
                DrawLine(GlobalPosition, GlobalPosition + vec, c);
            }
        }
    }
}
