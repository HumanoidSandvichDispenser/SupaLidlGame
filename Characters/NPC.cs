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

    [Export]
    public bool ShouldMoveWhenUsingItem { get; set; } = true;

    [Export]
    public State.Thinker.ThinkerStateMachine ThinkerStateMachine { get; set; }

    public bool ShouldMove { get; set; } = true;

    public bool CanAttack { get; set; } = true;

    public Vector2 LastSeenPosition { get; set; }

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

        Inventory.UsedItem += (Items.Item item) =>
        {
            if (item is Items.Weapon)
            {
                if (AttackAnimation is not null)
                {
                    AttackAnimation.TryPlay("attack");
                }
            }
        };
    }

    /// <summary>
    /// Finds the NPC's best character to target.
    /// </summary>
    public virtual Character FindBestTarget()
    {
        float bestScore = float.MaxValue;
        Character bestChar = null;
        // NOTE: this relies on all Characters being under the Entities node
        foreach (Node node in GetParent().GetChildren())
        {
            if (node is Character character)
            {
                bool isFriendly = character.Faction == Faction;
                if (isFriendly || character.Health <= 0)
                {
                    continue;
                }

                float score = 0;
                score -= Position.DistanceTo(character.Position);

                if (score < bestScore)
                {
                    bestScore = score;
                    bestChar = character;
                }
            }
        }
        return bestChar;
    }

    public override void _Process(double delta)
    {
        ThinkerStateMachine.Process(delta);
        base._Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        ThinkerStateMachine.PhysicsProcess(delta);
        base._PhysicsProcess(delta);
    }
}
