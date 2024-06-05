using Godot;

namespace SupaLidlGame.Extensions;

public static class TimerExtensions
{
    public static void Restart(this Timer timer, double timeSec = -1)
    {
        timer.Stop();
        timer.Start(timeSec);
    }
}
