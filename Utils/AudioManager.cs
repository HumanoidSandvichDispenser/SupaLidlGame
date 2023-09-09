using Godot;
using System.Collections.Generic;

namespace SupaLidlGame.Utils;

public sealed partial class AudioManager : Node
{
    public enum Layer
    {
        Ambient,
        BackgroundMusic,
        ActiveMusic,
    };

    //private Array<AudioStreamPlayer> _players;

    private Dictionary<Layer, AudioStreamPlayer> _players;

    public override void _Ready()
    {
        _players = new Dictionary<Layer, AudioStreamPlayer>();
        _players.Add(Layer.Ambient, null);
        _players.Add(Layer.BackgroundMusic, null);
        _players.Add(Layer.ActiveMusic, null);
    }

    public void PlayAmbient(AudioStream stream)
    {
        StopPlayback(Layer.Ambient);
        _players[Layer.Ambient] = CreateAudioStreamPlayer(stream);
        FadeIn(_players[Layer.Ambient]);
    }

    public void PlayBackground(AudioStream stream)
    {
        StopPlayback(Layer.BackgroundMusic);
        _players[Layer.BackgroundMusic] = CreateAudioStreamPlayer(stream);
        if (!IsLayerValid(Layer.ActiveMusic))
        {
            FadeIn(_players[Layer.BackgroundMusic]);
        }
    }

    public void StopBackground()
    {
        StopPlayback(Layer.BackgroundMusic);
    }

    public void PlayActive(AudioStream stream)
    {
        _players[Layer.ActiveMusic] = CreateAudioStreamPlayer(stream);
        _players[Layer.ActiveMusic].Play();
        if (IsLayerValid(Layer.BackgroundMusic))
        {
            Pause(Layer.BackgroundMusic);
        }
    }

    public void StopActive()
    {
        StopPlayback(Layer.ActiveMusic);
        if (IsLayerValid(Layer.BackgroundMusic))
        {
            if (_players[Layer.BackgroundMusic].StreamPaused)
            {
                Unpause(Layer.BackgroundMusic);
            }
            else
            {
                _players[Layer.BackgroundMusic].Play();
            }
        }
    }

    public void Play(AudioStream stream, Layer layer)
    {
        switch (layer)
        {
            case Layer.Ambient:
                PlayAmbient(stream);
                break;
            case Layer.BackgroundMusic:
                PlayBackground(stream);
                break;
            case Layer.ActiveMusic:
                PlayActive(stream);
                break;
        }
    }

    public void Stop(Layer layer)
    {
        switch (layer)
        {
            case Layer.Ambient:
            case Layer.BackgroundMusic:
                StopPlayback(layer);
                break;
            case Layer.ActiveMusic:
                StopActive();
                break;
        }
    }

    private void Pause(Layer layer)
    {
        var player = _players[layer];

        if (player is null || !IsInstanceValid(player))
        {
            return;
        }

        FadeOut(player, pause: true);
    }

    private void Unpause(Layer layer)
    {
        var player = _players[layer];

        if (player is null || !IsInstanceValid(player))
        {
            return;
        }

        FadeIn(player, unpause: true);
    }

    private void StopPlayback(Layer layer)
    {
        var player = _players[layer];

        if (player is null || !IsInstanceValid(player))
        {
            return;
        }

        FadeOut(player, kill: true);
        _players[layer] = null;
    }

    private AudioStreamPlayer CreateAudioStreamPlayer(AudioStream stream)
    {
        var player = new AudioStreamPlayer();
        AddChild(player);
        player.Stream = stream;
        return player;
    }

    private void FadeOut(
        AudioStreamPlayer player,
        double time = 1.0,
        bool kill = false,
        bool pause = false)
    {
        var tween = player.GetTree().CreateTween().BindNode(player);
        tween.TweenProperty(player, "volume_db", 0, time);
        if (kill)
        {
            tween.TweenCallback(Callable.From(player.QueueFree));
        }
        else if (pause)
        {
            tween.TweenCallback(
                Callable.From(() => player.StreamPaused = true));
        }
    }

    private void FadeIn(AudioStreamPlayer player,
            double time = 1.0,
            bool unpause = false)
    {
        if (unpause)
        {
            player.StreamPaused = false;
        }
        else
        {
            player.Play();
        }
        player.VolumeDb = 0;
        var tween = player.GetTree().CreateTween().BindNode(player);
        tween.TweenProperty(player, "volume_db", 1, time);
    }

    public bool IsLayerValid(Layer layer)
    {
        var player = _players[layer];
        return player is not null && IsInstanceValid(player);
    }
}
