using System;
using Godot;

namespace SupaLidlGame.UI;

public partial class FramerateCounter : Label
{
    public override void _Process(double delta)
    {
        Text = $"{Math.Round(Engine.GetFramesPerSecond())} FPS";
    }
}
