using Godot;
using Godot.Collections;
using System.Linq;

namespace SupaLidlGame.Debug;

public partial class Inspector : Node
{
    private Node _target;

    private Array<Dictionary> _props;

    private Dictionary<string, Label> _labels;

    public Node Target
    {
        get => _target;
        set
        {
            _target = value;
            UpdateTarget();
        }
    }

    public override void _Process(double delta)
    {
        foreach (var kv in _labels)
        {
            string propName = kv.Key;
            var node = kv.Value;
            node.Text = _target.Get(propName).ToString();
        }
    }

    public void UpdateTarget()
    {
        foreach (var kv in _labels)
        {
            var node = kv.Value;
            node.QueueFree();
        }
        _labels.Clear();
        LoadAllProperties();
    }

    public void LoadAllProperties()
    {
        _props = _target.GetPropertyList();
        foreach (var kv in _props)
        {
        }
    }
}
