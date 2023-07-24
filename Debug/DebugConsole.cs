using Godot;

namespace SupaLidlGame.Debug;

public partial class DebugConsole : Node
{
    public void SetProp(
        Utils.World world,
        string entityName,
        string property,
        string value)
    {
        var ent = world.CurrentMap.Entities.GetNodeOrNull(entityName);
        if (ent is not null)
        {
            ent.Set(property, value);
        }
    }

    public string CallMethod(
        Utils.World world,
        string entityName,
        string method,
        string[] args)
    {
        var ent = world.CurrentMap.Entities.GetNodeOrNull(entityName);
        if (ent is not null)
        {
            var returnValue = ent.Call(method, args);
            return returnValue.ToString();
        }
        return "";
    }
}
