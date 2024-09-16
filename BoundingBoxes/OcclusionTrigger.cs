using Godot;
using System.Linq;
using System.Collections.Generic;

namespace SupaLidlGame.BoundingBoxes;

public partial class OcclusionTrigger : Area2D
{
    [Export]
    public double FadeDuration { get; set; }

    [Export]
    public Godot.Collections.Array<string> Groups { get; set; }

    private Tween _tween = null;

    public override void _Ready()
    {
        Connect(SignalName.BodyEntered, new Callable(this, nameof(OnBodyEntered)));
        Connect(SignalName.BodyExited, new Callable(this, nameof(OnBodyExited)));
    }

    private IEnumerable<CanvasItem> GetCanvasItems()
    {
        IEnumerable<IEnumerable<CanvasItem>> pack()
        {
            foreach (string group in Groups)
            {
                var nodes = GetTree().GetNodesInGroup(group)
                    .OfType<CanvasItem>();

                yield return nodes;
            }
        }

        return pack().SelectMany(e => e);
    }

    private void OnBodyEntered(Node2D _)
    {
        if (IsInstanceValid(_tween))
        {
            _tween.Kill();
        }

        _tween = GetTree().CreateTween();
        _tween.SetParallel();

        foreach (var node in GetCanvasItems())
        {
            _tween.TweenProperty(node, "modulate", Colors.Transparent, FadeDuration);
        }
    }

    private void OnBodyExited(Node2D _)
    {
        if (IsInstanceValid(_tween))
        {
            _tween.Kill();
        }

        _tween = GetTree().CreateTween();
        _tween.SetParallel();

        foreach (var node in GetCanvasItems())
        {
            _tween.TweenProperty(node, "modulate", Colors.White, FadeDuration);
        }
    }
}
