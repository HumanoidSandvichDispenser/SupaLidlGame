using Godot;

namespace SupaLidlGame.Utils;

[GlobalClass]
public partial class ResourcePath : Resource
{
    private static Godot.Collections.Dictionary<string, Resource> _loaded;

    static ResourcePath()
    {
        _loaded = new();
    }

    [Export(Godot.PropertyHint.File)]
    public string Path { get; set; }

    public T Load<T>() where T : Resource
    {
        return GD.Load<T>(Path);
    }
}
