using Godot;

namespace SupaLidlGame.Utils;

[GlobalClass]
public partial class ScenePath : ResourcePath
{
    private static Godot.Collections.Dictionary<string, PackedScene> _loaded;

    static ScenePath()
    {
        _loaded = new();
    }

    public PackedScene Load(bool useCached = false)
    {
        if (useCached && _loaded.ContainsKey(Path))
        {
            return _loaded[Path];
        }

        // add scene to loaded to not have to reload scene when called again
        var scene = base.Load<PackedScene>();

        if (useCached)
        {
            _loaded.Add(Path, scene);
        }

        return scene;
    }

    public T Instantiate<T>() where T : Node
    {
        return Load().Instantiate<T>();
    }
}
