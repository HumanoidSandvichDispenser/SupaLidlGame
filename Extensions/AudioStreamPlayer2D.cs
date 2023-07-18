using Godot;
using GodotUtilities;
using System;

namespace SupaLidlGame.Extensions;

public static class AudioStreamPlayer2DExtensions
{
    public static AudioStreamPlayer2D Clone(
        this AudioStreamPlayer2D audio)
    {
        var clone = audio.Duplicate() as AudioStreamPlayer2D;
        clone.Finished += () =>
        {
            clone.QueueFree();
        };
        return clone;
    }

    public static AudioStreamPlayer2D On(
        this AudioStreamPlayer2D audio,
        Node parent)
    {
        var clone = audio.Clone();
        parent.AddChild(clone);
        clone.GlobalPosition = audio.GlobalPosition;
        return clone;
    }

    public static AudioStreamPlayer2D OnWorld(
        this AudioStreamPlayer2D audio)
    {
        var world = audio.GetTree().Root.GetNode("World/TileMap");
        if (world is null)
        {
            throw new NullReferenceException("World does not exist");
        }
        var clone = audio.On(world);
        clone.GlobalPosition = audio.GlobalPosition;
        return clone;
    }

    public static AudioStreamPlayer2D At(
        this AudioStreamPlayer2D audio,
        Vector2 globalPosition)
    {
        var world = audio.GetAncestor<Scenes.Map>();
        if (world is null)
        {
            throw new NullReferenceException("World does not exist");
        }

        var parent = new Node2D();
        world.AddChild(parent);
        parent.GlobalPosition = globalPosition;

        var clone = audio.On(world);
        clone.Finished += () =>
        {
            parent.QueueFree();
        };
        return clone;
    }

    public static AudioStreamPlayer2D PlayOneShot(
        this AudioStreamPlayer2D audio,
        float fromPosition = 0)
    {
        audio.Finished += () => audio.QueueFree();
        audio.Play(fromPosition);
        return audio;
    }

    public static AudioStreamPlayer2D WithPitchDeviation(
        this AudioStreamPlayer2D audio,
        float deviation)
    {
        audio.PitchScale = (float)GD.Randfn(audio.PitchScale, deviation);
        return audio;
    }
}
