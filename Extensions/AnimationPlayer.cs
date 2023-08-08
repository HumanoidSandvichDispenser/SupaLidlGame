using Godot;

namespace SupaLidlGame.Extensions;

public static class AnimationPlayerExtensions
{
    public static bool TryPlay(this AnimationPlayer player, string name)
    {
        var hasAnimation = player.HasAnimation(name);
        if (hasAnimation)
        {
            player.Play(name);
        }
        return hasAnimation;
    }

    public static bool TryQueue(this AnimationPlayer player, string name)
    {
        var hasAnimation = player.HasAnimation(name);
        if (hasAnimation)
        {
            player.Queue(name);
        }
        return hasAnimation;
    }
}
