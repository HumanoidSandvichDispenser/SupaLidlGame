using Godot;
using Godot.Collections;
using GodotUtilities.Collections;
using System.Linq;
using SupaLidlGame.Utils;

namespace SupaLidlGame.Items;

[GlobalClass]
public partial class Shop : Resource, IItemCollection<ShopEntry>
{
    [Export]
    protected Godot.Collections.Array<ShopEntry> Entries { get; private set; }

    public System.Collections.Generic.IEnumerable<ItemMetadata> GetItems()
    {
        var mapState = World.Instance.GlobalState.MapState;
        return Entries
            .Where(
                (entry) =>
                {
                    var condition = entry.MapStateCondition;
                    if (string.IsNullOrEmpty(condition))
                    {
                        return true;
                    }
                    return mapState.GetBoolean(condition) ?? false;
                }
            )
            .Select((entry) => entry.Item);
    }

    public bool Add(ShopEntry entry)
    {
        Entries.Add(entry);
        return true;
    }

    public bool Remove(ShopEntry entry)
    {
        return Entries.Remove(entry);
    }

    public int Capacity => Entries.Count;
}
