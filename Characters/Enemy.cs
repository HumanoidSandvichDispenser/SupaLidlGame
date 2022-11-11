using Godot;
using System;

namespace SupaLidlGame.Characters
{
    public partial class Enemy : NPC
    {
        public override void _Ready()
        {
            base._Ready();
        }

        public override void _Process(double delta)
        {
            if ((_thinkTimeElapsed -= delta) <= 0)
            {
                _thinkTimeElapsed = 0.25;
                Think();
            }

            //Direction = (Target.GlobalPosition - GlobalPosition).Normalized();
            base._Process(delta);
        }

        public override void _Draw()
        {
            for (byte i = 0; i < 16; i++)
            {
                Vector2 diff = WeightVec(i) * 128;
                Color c = _dirIdx == i ? new Color(0, 255, 64) : new Color(0, 128, 24);
                DrawLine(Vector2.Zero, diff, c, 2.0f);
            }

            base._Draw();
        }

        private Vector2 WeightVec(int idx)
        {
            //GD.Print(_weights[idx]);
            return WeightVecNorm(idx) * _weights[idx];
        }

        private Vector2 WeightVecNorm(int idx)
        {
            // sin(2pix/16)
            float x = Mathf.Cos(Mathf.Pi * idx / 8);
            float y = Mathf.Sin(Mathf.Pi * idx / 8);
            return new Vector2(x, y);
        }

        private Character FindBestTarget()
        {
            float bestDist = float.MaxValue;
            Character bestChar = null;
            foreach (Node node in GetParent().GetChildren())
            {
                if (node != this && node is Character character)
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

        private void Think()
        {
            Target = FindBestTarget();
            float bestWeight = 0;

            Vector2 want = GlobalPosition.DirectionTo(Target.GlobalPosition);
            float dist = GlobalPosition.DistanceSquaredTo(Target.GlobalPosition);
            for (byte i = 0; i < 16; i++)
            {
                Vector2 dir = WeightVecNorm(i);
                // if close enough, _weights[i] will be instead calculated dot to a perpendicular weight
                if (dist < 16384)
                {
                    Vector2 dirA = WeightVecNorm((i + 4) % 16);
                    Vector2 dirB = WeightVecNorm((i - 4) % 16);
                    float dot = Mathf.Max(dirA.Dot(want) + dirA.Dot(Direction),
                                          dirB.Dot(want) + dirB.Dot(Direction));
                    _weights[i] = (dot + 1) / 2;
                }
                else
                {
                    _weights[i] = (dir.Dot(want) + 1) / 2;
                }

                // check each weight with a raycast to see if it will hit any object
                // if it hits an object, subtract the weight by how close the ray is
                if (_weights[i] > 0)
                {
                    GD.Print("casting...");
                    var spaceState = GetWorld2d().DirectSpaceState;
                    var args = new PhysicsRayQueryParameters2D();
                    args.From = GlobalPosition;
                    args.To = GlobalPosition + dir * 256 * _weights[i];
                    args.CollideWithBodies = true;
                    args.Exclude.Add(this.GetRid());
                    var result = spaceState.IntersectRay(args);
                    GD.Print(result.Count);
                    if (result.Count > 0)
                    {
                        var pos = result["position"].AsVector2();
                        var sub = pos.DistanceTo(GlobalPosition + dir);
                        _weights[i] -= sub;
                        GD.Print("hit!");
                    }
                }

                if (_weights[i] > bestWeight)
                {
                    bestWeight = _weights[i];
                    _dirIdx = i;
                }

                //GD.Print(_weights[i]);
            }
            Direction = WeightVecNorm((byte)_dirIdx);
            QueueRedraw();
        }
    }
}
