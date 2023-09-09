using Godot;
using System.Collections.Generic;

namespace SupaLidlGame.Audio;

public sealed partial class AudioManager : Node
{
    private ManagedAudioPlayer _ambientPlayer;

    private List<ManagedAudioPlayer> _bgPlayers;

    public enum Layer
    {
        Ambient,
        Background
    }

    public AudioManager()
    {
        _bgPlayers = new();
        for (int i = 0; i < 4; i++)
        {
            _bgPlayers.Add(null);
        }
    }

    public ManagedAudioPlayer CreateAudioPlayer(AudioStream stream)
    {
        var player = new ManagedAudioPlayer();
        player.Stream = stream;
        AddChild(player);
        return player;
    }

    /// <summary>
    /// Plays ambient audio. Ambient audio will play alongside the current
    /// background audio and will replace the current ambient audio.
    /// </summary>
    public void PlayAmbient(AudioStream stream, float fade = 1)
    {
        if (IsPlayerValid(_ambientPlayer))
        {
            _ambientPlayer.FadeOut(fade, true);
        }
        _ambientPlayer = CreateAudioPlayer(stream);
        _ambientPlayer.Play();
    }

    /// <summary>
    /// Plays background audio. Background audio will play if no sublayer above
    /// is playing, pause all sublayers below, and replace the current sublayer
    /// if it is playing.
    /// </summary>
    public void PlayBackground(AudioStream stream, int sublayer = 0, float fade = 1)
    {
        // stop current sublayer if playing
        if (IsPlayerValid(_bgPlayers[sublayer]))
        {
            _bgPlayers[sublayer].FadeOut(fade, true);
        }
        _bgPlayers[sublayer] = CreateAudioPlayer(stream);

        // pause all sublayers below this
        for (int i = 0; i < sublayer; i++)
        {
            if (IsPlayerValid(_bgPlayers[i]))
            {
                if (!_bgPlayers[i].StreamPaused)
                {
                    _bgPlayers[i]?.FadeOut(fade, pause: true);
                }
            }
        }

        // if a sublayer above is not playing then play
        if (!IsSublayerAboveValid(sublayer))
        {
            _bgPlayers[sublayer].Play();
        }
    }

    public void StopBackground(int sublayer, float fade = 1)
    {
        // kill current sublayer
        _bgPlayers[sublayer]?.FadeOut(fade, true);

        // if sublayer above is not plyaing, play lowest sublayer
        if (!IsSublayerAboveValid(sublayer))
        {
            for (int i = sublayer - 1; i >= 0; i--)
            {
                if (IsPlayerValid(_bgPlayers[i]))
                {
                    _bgPlayers[i].FadeIn(fade, unpause: true);
                    break;
                }
            }
        }
    }

    private bool IsSublayerAboveValid(int sublayer)
    {
        for (int i = sublayer + 1; i < 4; i++)
        {
            if (IsPlayerValid(_bgPlayers[i]))
            {
                return true;
            }
        }

        return false;
    }

    private bool IsPlayerValid(ManagedAudioPlayer player)
    {
        return player is not null && IsInstanceValid(player) && !player.IsDead;
    }
}
