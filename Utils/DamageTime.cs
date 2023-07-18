using Godot;

namespace SupaLidlGame.Utils;

public partial class DamageTime : Node
{
    private double _duration = 0.1;

    private double _currentDuration = 0;

    private double _startValue = 0;

    public override void _Ready()
    {
        if (GetParent() is Characters.Player player)
        {
            player.Hurt += PlayerHurt;
        }
    }

    private void PlayerHurt(Events.HealthChangedArgs args)
    {
        if (args.Damage > 10)
        {
            // temp
            //float strength = 0.8f;
            //_startValue = 1 - strength;
            //_currentDuration = 0;
            _currentDuration = _duration;
            Engine.TimeScale = 0.1;
        }
    }

    public override void _Process(double delta)
    {
        if ((_currentDuration -= delta) < 0)
        {
            Engine.TimeScale = 1;
        }
    }
}
