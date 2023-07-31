#define DEBUG_NPC

using Godot;
using SupaLidlGame.Extensions;
using SupaLidlGame.Items;
using System;

namespace SupaLidlGame.Characters;

public partial class NPC : Character
{
    /// <summary>
    /// Time in seconds it takes for the NPC to think FeelsDankCube
    /// </summary>
    public const float ThinkTime = 0.125f;

    public float[] Weights => _weights;

    protected float _preferredWeightDistance = 64.0f;
    protected float _maxWeightDistance = 8.0f;
    protected float _preferredWeightDistanceSq = 4096.0f;
    protected float _maxWeightDistanceSq = 64.0f;

    [Export]
    public float PreferredWeightDistance
    { 
        get => _preferredWeightDistance;
        protected set
        {
            _preferredWeightDistance = value;
            _preferredWeightDistanceSq = value * value;
        }
    }

    [Export]
    public float MaxWeightDistance
    { 
        get => _maxWeightDistance;
        protected set
        {
            _maxWeightDistance = value;
            _maxWeightDistanceSq = value * value;
        }
    }

    [Export]
    public Items.Item DefaultSelectedItem { get; set; }

    public bool ShouldMove { get; set; } = true;

    public bool CanAttack { get; set; } = true;

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

        if (DefaultSelectedItem is not null)
        {
            Inventory.SelectedItem = DefaultSelectedItem;
        }
    }

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
#endif

        base._Draw();
    }

    public virtual Character FindBestTarget()
    {
        float bestDist = float.MaxValue;
        Character bestChar = null;
        foreach (Node node in GetParent().GetChildren())
        {
            if (node is Character character)
            {
                bool isFriendly = character.Faction == Faction;
                if (isFriendly || character.Health <= 0)
                {
                    continue;
                }

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
#if DEBUG_NPC
            QueueRedraw();
#endif
        }

        if (ShouldMove)
        {
            Direction = _weightDirs[_bestWeightIdx];
        }
        else
        {
            Direction = Vector2.Zero;
        }
    }

    public void UpdateWeights(Vector2 pos)
    {
        // FIXME: TODO: remove all the spaghetti
        Vector2 dir = Target.Normalized();
        float distSq = GlobalPosition.DistanceSquaredTo(pos);

        var spaceState = GetWorld2D().DirectSpaceState;
        var exclude = new Godot.Collections.Array<Godot.Rid>();
        exclude.Add(this.GetRid());

        // calculate weights based on distance
        for (int i = 0; i < 16; i++)
        {
            float directDot = _weightDirs[i].Dot(dir);
            // clamp dot from [-1, 1] to [0, 1]
            directDot = (directDot + 1) / 2;

            float strafeDot = Math.Abs(_weightDirs[i].Dot(dir.Clockwise90()));
            float currDirDot = (_weightDirs[i].Dot(Direction) + 1) / 16;
            strafeDot = Mathf.Pow((strafeDot + 1) / 2, 2) + currDirDot;

            // favor strafing when getting closer
            if (distSq > _preferredWeightDistanceSq)
            {
                _weights[i] = directDot;
            }
            else if (distSq > _maxWeightDistanceSq)
            {
                float dDotWeight = Mathf.Sqrt(distSq / 4096);
                float sDotWeight = 1 - dDotWeight;
                _weights[i] = (dDotWeight * directDot) +
                    (sDotWeight * strafeDot);
            }
            else
            {
                _weights[i] = strafeDot;
            }
        }

        // subtract weights that collide
        for (int i = 0; i < 16; i++)
        {
            var rayParams = new PhysicsRayQueryParameters2D
            {
                Exclude = exclude,
                CollideWithBodies = true,
                From = GlobalPosition,
                To = GlobalPosition + (_weightDirs[i] * 24),
                CollisionMask = 1 + 2 + 16
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

    protected virtual void Think()
    {
        // TODO: the entity should wander if it doesn't find a best target
        Character bestTarget = FindBestTarget();
        if (bestTarget is not null)
        {
            Vector2 pos = FindBestTarget().GlobalPosition;
            Target = pos - GlobalPosition;
            Vector2 dir = Target;
            float dist = GlobalPosition.DistanceSquaredTo(pos);
            UpdateWeights(pos);

            if (dist < 1600 && CanAttack)
            {
                if (Inventory.SelectedItem is Weapon weapon)
                {
                    UseCurrentItem();
                }
            }
        }
    }
}
