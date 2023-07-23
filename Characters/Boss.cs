using Godot;
using GodotUtilities;

namespace SupaLidlGame.Characters;

public abstract partial class Boss : Enemy
{
    [Export]
    public State.NPC.NPCStateMachine BossStateMachine { get; set; }

    [Export]
    public string BossName { get; set; }

    public abstract int Intensity { get; }

    private bool _isActive;

    [Export]
    public bool IsActive
    {
        get => _isActive;
        set
        {
            _isActive = value;

            // register or deregister ourselves when we are active/inactive
            this.GetAncestor<Utils.World>()
                .RegisterBoss(_isActive ? this : null);
        }
    }
}
