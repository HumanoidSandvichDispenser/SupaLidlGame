using Godot;
using GodotUtilities;

namespace SupaLidlGame.State.NPC.Doc;

public partial class DocAttackState : NPCState
{
    private Scenes.Map _map;
    private Utils.World _world;

    private double _currentDuration = 0;
    private double _currentAttackDuration = 0;

    [Export]
    public double Duration { get; set; }

    [Export]
    public double AttackDuration { get; set; }

    [Export]
    public PackedScene Projectile { get; set; }

    [Export]
    public DocExitState ExitState { get; set; }

    private float _intensity = 1;

    public override NPCState Enter(IState<NPCState> previousState)
    {
        _map = this.GetAncestor<Scenes.Map>();
        _world = this.GetAncestor<Utils.World>();
        _currentDuration = Duration;
        _currentAttackDuration = AttackDuration;
        return null;
    }

    public override void Exit(IState<NPCState> nextState)
    {

    }

    private void SpawnProjectile(Vector2 position, Vector2 direction)
    {
        var projectile = _map.SpawnEntity<Entities.Projectile>(Projectile);
        projectile.Hitbox.Faction = NPC.Faction;
        // global position is (from npc to player) * 2 = (2 * npc) - player
        //projectile.GlobalPosition = 2 * NPC.GlobalPosition - playerPos;
        projectile.GlobalPosition = position;
        projectile.Direction = direction;
        projectile.GlobalRotation = direction.Angle();
        projectile.Delay = 1 / _intensity;
    }

    public override NPCState Process(double delta)
    {
        if ((_currentDuration -= delta) <= 0)
        {
            return ExitState;
        }

        if (NPC.Health < 500)
        {
            _intensity = 2;
        }

        if (NPC.Health < 250)
        {
            _intensity = 3;
        }

        if ((_currentAttackDuration -= delta) <= 0)
        {
            var player = _world.CurrentPlayer;
            var playerPos = player.GlobalPosition;
            Vector2 position1 = 2 * NPC.GlobalPosition - playerPos;
            Vector2 position2 = 2 * playerPos - NPC.GlobalPosition;
            Vector2 direction1 = position1.DirectionTo(playerPos);
            Vector2 direction2 = -direction1;
            SpawnProjectile(position1, direction1);
            SpawnProjectile(position2, direction2);
            _currentAttackDuration = AttackDuration / _intensity;
        }

        return null;
    }
}
