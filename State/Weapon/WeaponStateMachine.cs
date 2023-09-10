using Godot;

namespace SupaLidlGame.State.Weapon;

public partial class WeaponStateMachine : StateMachine<WeaponState>
{
    [Export]
    public override WeaponState InitialState { get; set; }

    /// <summary>
    /// List of initial states when an item is being used. This should also
    /// include alt use states.
    /// </summary>
    [Export]
    public Godot.Collections.Array<NodePath> UsedItemStates { get; protected set; }

    [Export]
    public Godot.Collections.Array<NodePath> UsedItemAltStates { get; protected set; }

    [Export]
    public Godot.Collections.Array<NodePath> DeusedItemStates { get; protected set; }

    public void Use()
    {
        var state = CurrentState.Use();
        if (state is WeaponState)
        {
            ChangeState(state);
        }
    }

    public void Deuse()
    {
        var state = CurrentState.Deuse();
        if (state is WeaponState)
        {
            ChangeState(state);
        }
    }

    public void UseAlt()
    {
        var state = CurrentState.UseAlt();
        if (state is WeaponState)
        {
            ChangeState(state);
        }
    }

    public void DeuseAlt()
    {
        var state = CurrentState.DeuseAlt();
        if (state is WeaponState)
        {
            ChangeState(state);
        }
    }

    public void Process(double delta)
    {
        var state = CurrentState.Process(delta);
        if (state is WeaponState s)
        {
            ChangeState(s);
        }
    }
}
