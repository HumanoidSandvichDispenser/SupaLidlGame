using Godot;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.State.Thinker;

public partial class CenturionAttackState : BlockAttackState
{
    [Export]
    public bool FollowTeammate { get; set; } = false;

    [Export]
    public NavigationAgent2D NavigationAgent { get; set; }

    [Export]
    public float PathfindingDistance { get; set; } = 64;

    protected Characters.Character _bestTeammate = null;

    public Characters.Character FindBestTeammate()
    {
        float bestScore = float.MaxValue;
        Characters.Character bestChar = null;
        var world = this.GetWorld();

        foreach (Node node in world.CurrentMap.Entities.GetChildren())
        {
            if (node != NPC && node is Characters.Character character)
            {
                if (!IsInstanceValid(character) || !character.IsAlive)
                {
                    continue;
                }

                if (character.Faction == NPC.Faction)
                {
                    float score = NPC.Position.DistanceTo(character.Position);
                    if (score < bestScore)
                    {
                        bestScore = score;
                        bestChar = character;
                    }
                }
            }
        }

        return _bestTeammate = bestChar;
    }

    public override ThinkerState Think()
    {
        base.Think();

        var teammate = FindBestTeammate();
        if (teammate is not null)
        {
            UpdateWeights(teammate.GlobalPosition);
            NavigationAgent.TargetPosition = teammate.GlobalPosition;
        }

        return null;
    }

    public override ThinkerState Process(double delta)
    {
        if (_bestTeammate is null)
        {
            return null;
        }

        float dist = NPC.DistanceTo(_bestTeammate);
        if (NPC.DistanceTo(_bestTeammate) < PathfindingDistance)
        {
            // move to weighted position
            return base.Process(delta);
        }
        return null;
    }

    public override ThinkerState PhysicsProcess(double delta)
    {
        if (_bestTeammate is not null)
        {
            float dist = NPC.DistanceTo(_bestTeammate);
            if (dist >= PathfindingDistance)
            {
                var nextPos = NavigationAgent.GetNextPathPosition();
                NPC.Direction = NPC.GlobalPosition.DirectionTo(nextPos);
            }
        }

        return base.PhysicsProcess(delta);
    }
}
