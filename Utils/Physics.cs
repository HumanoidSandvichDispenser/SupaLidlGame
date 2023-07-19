using Godot;

namespace SupaLidlGame.Utils;

public static class Physics
{
    /// <summary>
    /// Returns the predicted position of a target after a time t at which a
    /// projectile is predicted to hit a target.
    /// </summary>
    public static Vector2 PredictNewPosition(
        Vector2 position,
        float speed,
        Vector2 targetPosition,
        Vector2 targetVelocity,
        out float time)
    {
        // relative velocity is sum of projectile's relative velocity when idle
        // plus its relative velocity when heading towards target (as origin)
        var relIdle = -targetVelocity;
        var relDir = (position - targetPosition).DirectionTo(Vector2.Zero);

        var relVel = relIdle + relDir * speed;
        var relSpeed = relVel.Length();

        GD.Print("Relative velocity: " + relVel);

        // get t = time to travel
        //       = (|s2| - |s1|)/(|v2| - |v1|)
        time = position.DistanceTo(targetPosition) / relSpeed;
        GD.Print("Time: " + time);

        // predict target's position after time t
        return targetPosition + targetVelocity * time;
    }
}
