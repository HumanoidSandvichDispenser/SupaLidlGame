using Godot;
using SupaLidlGame.Extensions;
using SupaLidlGame.Items;
using System;

namespace SupaLidlGame.Characters
{
    public partial class NPC : Character
    {
        /// <summary>
        /// Time in seconds it takes for the NPC to think FeelsDankCube
        /// </summary>
        public const float ThinkTime = 0.125f;

        public float[] Weights => _weights;

        [Export]
        public float PreferredDistance { get; protected set; } = 64.0f;

        protected float[] _weights = new float[16];
        protected int _bestWeightIdx;
        protected double _thinkTimeElapsed = 0;
        protected Vector2 _blockingDir;
        protected static readonly Vector2[] _weightDirs = new Vector2[16];

        static NPC()
        {
            for (int i = 0; i < 16; i++)
            {
                float y = Mathf.Sin(Mathf.Pi * i * 2 / 16);
                float x = Mathf.Cos(Mathf.Pi * i * 2 / 16);
                _weightDirs[i] = new Vector2(x, y);
            }
        }

        public override void _Ready()
        {
            base._Ready();
            Array.Fill(_weights, 0);
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
#if DEBUG
            for (int i = 0; i < 16; i++)
            {
                Vector2 vec = _weightDirs[i] * _weights[i] * 32;
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
            /*
            DrawLine(Vector2.Zero, Direction * 32, new Color(0, 1, 0));
            DrawLine(Vector2.Zero, Target * 32, Colors.Blue);
            DrawLine(Vector2.Zero, _blockingDir, Colors.Red, 2);
            */
#endif

            base._Draw();
        }

        protected virtual Character FindBestTarget()
        {
            float bestDist = float.MaxValue;
            Character bestChar = null;
            foreach (Node node in GetParent().GetChildren())
            {
                if (node is Character character && character.Faction != Faction)
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
                QueueRedraw();
            }

            //Direction = GetDirection(Target);
            Direction = _weightDirs[_bestWeightIdx];
        }

        public void UpdateWeights(Vector2 pos)
        {
            // FIXME: TODO: remove all the spaghetti
            Vector2 dir = Target.Normalized();
            float dist = GlobalPosition.DistanceSquaredTo(pos);

            var spaceState = GetWorld2d().DirectSpaceState;
            var exclude = new Godot.Collections.Array<RID>();
            exclude.Add(this.GetRid());

            for (int i = 0; i < 16; i++)
            {
                float directDot = _weightDirs[i].Dot(dir);
                // clamp dot from [-1, 1] to [0, 1]
                directDot = (directDot + 1) / 2;

                float strafeDot = Math.Abs(_weightDirs[i].Dot(dir.Clockwise90()));
                float currDirDot = (_weightDirs[i].Dot(Direction) + 1) / 16;
                strafeDot = Mathf.Pow((strafeDot + 1) / 2, 2) + currDirDot;

                if (dist > 4096)
                {
                    _weights[i] = directDot;
                }
                else if (dist > 64)
                {
                    float dDotWeight = Mathf.Sqrt(dist / 4096);
                    float sDotWeight = 1 - dDotWeight;
                    _weights[i] = (dDotWeight * directDot) + (sDotWeight * strafeDot);
                }
                else
                {
                    _weights[i] = strafeDot;
                }

            }

            for (int i = 0; i < 16; i++)
            {
                var rayParams = new PhysicsRayQueryParameters2D
                {
                    Exclude = exclude,
                    CollideWithBodies = true,
                    From = GlobalPosition,
                    To = GlobalPosition + (_weightDirs[i] * 24),
                    CollisionMask = 1 + 8
                };

                var result = spaceState.IntersectRay(rayParams);

                // if we hit something
                if (result.Count > 0)
                {
                    // then we subtract the value of this from the other weights
                    float oldWeight = _weights[i];
                    for (int j = 0; j < 16; j++)
                    {
                        if (i == j)
                        {
                            _weights[i] = 0;
                        }
                        else
                        {
                            float dot = _weightDirs[i].Dot(_weightDirs[j]);
                            _weights[j] -= _weights[j] * dot;
                        }
                    }
                }
            }


            float bestWeight = 0;
            for (int i = 0; i < 16; i++)
            {
                if (_weights[i] > bestWeight)
                {
                    _bestWeightIdx = i;
                    bestWeight = _weights[i];
                }
            }
        }

        public Vector2 GetDirection(Vector2 towards)
        {
            float directWeight;
            float strafeWeight;

            float dist = towards.Length();

            Vector2 directDir = towards.Normalized();
            Vector2 strafeDir;
            float crossProduct = towards.Cross(Direction);

            // strafeDir is either counter clockwise or clockwise depending on
            // the direction the NPC is already traveling

            // enemies might rapidly change direction if compared to 0
            if (crossProduct < -1)
            {
                strafeDir = directDir.Counterclockwise90();
            }
            else
            {
                strafeDir = directDir.Counterclockwise90();
                //strafeDir = directDir.Clockwise90();
            }

            // weights approach 1
            // dy/dx = 1 - y
            // y = 1 - e^(-x)

            directWeight = 1 - Mathf.Pow(Mathf.E, -(dist / PreferredDistance));
            strafeWeight = 1 - directWeight;

            /*
            Vector2 midpoint = (strafeDir * strafeWeight)
                .Midpoint(directDir * directWeight);
            */
            Vector2 midpoint = (directDir * directWeight)
                .Midpoint(strafeDir * strafeWeight);

            _blockingDir = GetBlocking();
            midpoint += _blockingDir;

            return midpoint.Normalized();
        }

        public Vector2 GetBlocking()
        {
            var spaceState = GetWorld2d().DirectSpaceState;
            int rayLength = 16;
            float[] weights = new float[16];
            Vector2[] rays = new Vector2[16];
            Vector2 net = Vector2.Zero;
            for (int i = 0; i < 16; i++)
            {
                // cast ray and gets its length
                // the length determines its strength

                // exclude itself from raycasts
                var exclude = new Godot.Collections.Array<RID>();
                exclude.Add(GetRid());

                var rayParams = new PhysicsRayQueryParameters2D
                {
                    CollisionMask = 9,
                    From = GlobalPosition,
                    To = _weightDirs[i] * rayLength,
                    Exclude = exclude
                };
                var result = spaceState.IntersectRay(rayParams);
                if (result.Count > 0)
                {
                    Vector2 position = (Vector2)result["position"];
                    float hitDist = GlobalPosition.DistanceTo(position);
                    //float hitDist = GlobalPosition.DistanceSquaredTo(position);
                    //float rayDist = Mathf.Pow(rayLength, 2);
                    //float weight = rayDist - hitDist;
                    float weight = rayLength - hitDist;
                    GD.Print(weight);
                    rays[i] = _weightDirs[i] * weight;
                    net += rays[i];
                }
                else
                {
                    rays[i] = Vector2.Zero;
                }
            }

            return net;
            //return -Vector2Extensions.Midpoints(rays);
        }

        protected virtual void Think()
        {
            Vector2 pos = FindBestTarget().GlobalPosition;
            Target = pos - GlobalPosition;//GlobalPosition.DirectionTo(pos);
            Vector2 dir = Target;
            float dist = GlobalPosition.DistanceSquaredTo(pos);
            UpdateWeights(pos);

            if (Target.LengthSquared() < 1024)
            {
                if (Inventory.SelectedItem is Weapon weapon)
                {
                    UseCurrentItem();
                }
            }
        }
    }
}
