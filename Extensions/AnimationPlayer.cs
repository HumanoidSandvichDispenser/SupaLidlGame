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

    public static bool TryPlayAny(this AnimationPlayer player,
        params string[] names)
    {
        return player.TryPlayAny(out var _, names);
    }

    public static bool TryPlayAny(this AnimationPlayer player,
        out string resultName, params string[] names)
    {
        foreach (string name in names)
        {
            if (player.TryPlay(name))
            {
                resultName = name;
                return true;
            }
        }
        resultName = default;
        return false;
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

    public static bool TryQueueAny(this AnimationPlayer player,
        params string[] names)
    {
        return player.TryQueueAny(out var _, names);
    }

    public static bool TryQueueAny(this AnimationPlayer player,
        out string resultName, params string[] names)
    {
        foreach (string name in names)
        {
            if (player.TryQueue(name))
            {
                resultName = name;
                return true;
            }
        }
        resultName = default;
        return false;
    }
}
