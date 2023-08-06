using Godot;
using SupaLidlGame.Extensions;
using System;

namespace SupaLidlGame.State.Thinker;

public partial class AttackState : ThinkerState
{
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
    public float MaxDistanceToTarget { get; set; }

    [Export]
    public ThinkerState PassiveState { get; set; }

    [Export]
    public ThinkerState PursueState { get; set; }

    protected float _preferredWeightDistance = 64.0f;
    protected float _maxWeightDistance = 8.0f;
    protected float _preferredWeightDistanceSq = 4096.0f;
    protected float _maxWeightDistanceSq = 64.0f;

    public float[] Weights => _weights;

    protected float[] _weights = new float[16];
    protected int _bestWeightIdx;
    protected static readonly Vector2[] _weightDirs = new Vector2[16];

    static AttackState()
    {
        for (int i = 0; i < 16; i++)
        {
            float y = Mathf.Sin(Mathf.Pi * i * 2 / 16);
            float x = Mathf.Cos(Mathf.Pi * i * 2 / 16);
            _weightDirs[i] = new Vector2(x, y);
        }
    }

    public void UpdateWeights(Vector2 pos)
    {
        // FIXME: TODO: remove all the spaghetti
        Vector2 dir = NPC.Target.Normalized();
        float distSq = NPC.GlobalPosition.DistanceSquaredTo(pos);

        var spaceState = NPC.GetWorld2D().DirectSpaceState;
        var exclude = new Godot.Collections.Array<Godot.Rid>();
        exclude.Add(NPC.GetRid());

        // calculate weights based on distance
        for (int i = 0; i < 16; i++)
        {
            float directDot = _weightDirs[i].Dot(dir);
            // clamp dot from [-1, 1] to [0, 1]
            directDot = (directDot + 1) / 2;

            float strafeDot = Math.Abs(_weightDirs[i].Dot(dir.Clockwise90()));
            float currDirDot = (_weightDirs[i].Dot(NPC.Direction) + 1) / 16;
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
                From = NPC.GlobalPosition,
                To = NPC.GlobalPosition + (_weightDirs[i] * 24),
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
                        if (dot >= 0)
                        {
                            _weights[j] -= _weights[j] * dot;
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
                _bestWeightIdx = i;
                bestWeight = _weights[i];
            }
        }
    }

    public override ThinkerState Think()
    {
        // TODO: the entity should wander if it doesn't find a best target
        var bestTarget = NPC.FindBestTarget();

        if (bestTarget is not null)
        {
            Vector2 pos = NPC.LastSeenPosition = bestTarget.GlobalPosition;
            NPC.Target = pos - NPC.GlobalPosition;
            Vector2 dir = NPC.Target;
            float dist = NPC.GlobalPosition.DistanceTo(pos);

            if (PursueState is not null)
            {
                // pursue the player if they can not be seen or is too far away
                if (dist > MaxDistanceToTarget || !NPC.HasLineOfSight(bestTarget))
                {
                    return PursueState;
                }
            }

            UpdateWeights(pos);

            if (dist < 40 && NPC.CanAttack)
            {
                if (NPC.Inventory.SelectedItem is Items.Weapon weapon)
                {
                    NPC.UseCurrentItem();
                }
            }
        }
        else
        {
            return PassiveState;
        }

        return base.Think();
    }

    public override ThinkerState Process(double delta)
    {
        if (!NPC.ShouldMove ||
            (!NPC.ShouldMoveWhenUsingItem && NPC.Inventory.IsUsingItem))
        {
            NPC.Direction = Vector2.Zero;
        }
        else
        {
            NPC.Direction = _weightDirs[_bestWeightIdx];
        }

        return base.Process(delta);
    }
}
