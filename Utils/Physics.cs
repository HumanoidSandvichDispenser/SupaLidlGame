using Godot;

namespace SupaLidlGame.Utils;

public static class Physics
{
    public static readonly float COS_30_DEG;

    public static readonly float COS_150_DEG;

    static Physics()
    {
        COS_30_DEG = Mathf.Cos(Mathf.DegToRad(30));
        COS_150_DEG = Mathf.Cos(Mathf.DegToRad(150));
    }

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

        // get t = time to travel
        //       = (|s2| - |s1|)/(|v2| - |v1|)
        time = position.DistanceTo(targetPosition) / relSpeed;

        // predict target's position after time t
        return targetPosition + targetVelocity * time;
    }
}
