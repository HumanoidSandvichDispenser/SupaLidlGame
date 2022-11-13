using Godot;
using System;

namespace SupaLidlGame.Characters
{
    public partial class NPC : Character
    {
        /// <summary>
        /// Time in seconds it takes for the NPC to think FeelsDankCube
        /// </summary>
        public const float ThinkTime = 0.25f;

        public float[] Weights => _weights;

        float[] _weights = new float[16];
        Vector2[] _weightDirs = new Vector2[16];
        int _bestWeightIdx;
        protected double _thinkTimeElapsed = 0;

        public override void _Ready()
        {
            base._Ready();
            Array.Fill(_weights, 0);
            for (int i = 0; i < 16; i++)
            {
                float y = Mathf.Sin(Mathf.Pi * i * 2 / 16);
                float x = Mathf.Cos(Mathf.Pi * i * 2 / 16);
                _weightDirs[i] = new Vector2(x, y);
            }
        }

        /*
        public override void _Process(double delta)
        {
            if ((_thinkTimeElapsed += delta) > ThinkTime)
            {
                _thinkTimeElapsed = 0;
                Think();
            }

            Direction = _weightDirs[_bestWeightIdx];
            //Direction = (Target.GlobalPosition - GlobalPosition).Normalized();
            base._Process(delta);
        }
        */

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
                else if (_weights[i] < 0)
                {
                    c = Colors.Red;
                    vec = -vec;
                }
                DrawLine(Vector2.Zero, vec, c);
            }

            base._Draw();
        }

        protected virtual Character FindBestTarget()
        {
            float bestDist = float.MaxValue;
            Character bestChar = null;
            foreach (Node node in GetParent().GetChildren())
            {
                if (node != this && node is Player character)
                {
                    float dist = Position.DistanceTo(character.Position);
                    if (dist < bestDist)
                    {
                        bestDist = dist;
                        bestChar = character;
                    }
                }
            }
            return bestChar;
        }

        public void ThinkProcess(double delta)
        {
            if ((_thinkTimeElapsed += delta) > ThinkTime)
            {
                _thinkTimeElapsed = 0;
                Think();
            }

            Direction = _weightDirs[_bestWeightIdx];
        }

        private void Think()
        {
            Vector2 pos = FindBestTarget().GlobalPosition;
            Vector2 dir = Target = GlobalPosition.DirectionTo(pos);
            float dist = GlobalPosition.DistanceSquaredTo(pos);

            for (int i = 0; i < 16; i++)
            {
                float directDot = _weightDirs[i].Dot(dir);
                directDot = (directDot + 1) / 2;

                // this dot product resembles values of sine rather than cosine
                // use it to weigh direction horizontally
                Vector2 rotatedDir = new Vector2(-dir.y, dir.x);
                float horizDot = Math.Abs(_weightDirs[i].Dot(rotatedDir));

                // this is a smaller weight so they are more likely to pick the
                // direction they are currently heading when choosing between two
                // horizontal weights
                float currDirDot = (_weightDirs[i].Dot(Direction) + 1) / 16;

                // square so lower values are even lower
                horizDot = Mathf.Pow((horizDot + 1) / 2, 2) + currDirDot;

                // "When will I use math in the real world" Clueful


                if (dist > 1024)
                {
                    _weights[i] = directDot;
                }
                else if (dist > 64)
                {
                    float directDotWeighting = (dist - 64) / 960;
                    float horizDotWeighting = 1 - directDotWeighting;

                    _weights[i] = (directDot * directDotWeighting) +
                        (horizDot * horizDotWeighting);
                }
                else
                {
                    // shorter than 64
                    _weights[i] = horizDot;
                }

                // now we shall subtract weights whose rays collide
                // with something

                {
                    var spaceState = GetWorld2d().DirectSpaceState;
                    var exclude = new Godot.Collections.Array<RID>();
                    exclude.Add(this.GetRid());
                    var rayParams = new PhysicsRayQueryParameters2D
                    {
                        Exclude = exclude,
                        CollideWithBodies = true,
                        From = GlobalPosition,
                        To = GlobalPosition + (_weightDirs[i] * 16)
                    };

                    var result = spaceState.IntersectRay(rayParams);

                    // if our ray cast hits something
                    if (result.Count > 0)
                    {
                        // then we subtract the dot product of other directions
                        for (int j = 0; j < 16; j++)
                        {
                            if (i == j)
                            {
                                _weights[i] = 0;
                            }
                            else
                            {
                                float dot = _weightDirs[i].Dot(_weightDirs[j]);
                                _weights[j] -= (dot + 1) / 2;
                            }
                        }
                    }
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
    }
}
