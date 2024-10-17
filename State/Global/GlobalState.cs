using Godot;
using SupaLidlGame.Utils;

namespace SupaLidlGame.State.Global;

public partial class GlobalState : Node, ISave
{
    [Export]
    public Progression Progression { get; set; }

    [Export]
    public MapState MapState { get; set; }

    [Export]
    public Stats Stats { get; set; }

    public static GlobalState Instance { get; private set; }

    [Export]
    public GameSettings Settings { get; set; }

    [Signal]
    public delegate void SummonBossEventHandler(string bossName);

    public GlobalState()
    {
        Progression = new();
        MapState = new();
        Stats = new();
        Settings = new();
    }

    public override void _Ready()
    {
        if (Instance != null)
        {
            throw new MultipleSingletonsException();
        }

        Instance = this;

        ProcessMode = ProcessModeEnum.Always;
        LoadSettings();
    }

    public override void _Notification(int what)
    {
        //if (what == NotificationWMCloseRequest)
        //{
        //    // TODO: quit prompt
        //    GetTree().Root
        //        .PropagateNotification((int)NotificationWMCloseRequest);
        //    SaveSettings();
        //}
    }

    private void LoadSettings()
    {
        if (ResourceLoader.Exists("user://settings.tres"))
        {
            Settings = ResourceLoader.Load<GameSettings>("user://settings.tres");
        }
        else
        {
            Settings = new GameSettings();
        }
    }

    public void SaveSettings()
    {
        ResourceSaver.Save(Settings, "user://settings.tres");
    }

    public void ImportFromSave(Save save)
    {
        Progression = save.Progression;
        MapState = save.MapState;
        Stats = save.Stats;

        var inventory = World.Instance.CurrentPlayer.Inventory;
        inventory.Items = Stats.Items;

        for (int i = 0; i < Stats.HotbarToItemIndexMap.Count; i++)
        {
            int itemIndex = Stats.HotbarToItemIndexMap[i];

            if (itemIndex >= 0)
            {
                inventory.SetHotbarIndexToItemIndex(i, itemIndex);
            }
        }
    }

    public void ExportToSave(Save save)
    {
        save.Progression = Progression;
        save.MapState = MapState;
        save.Stats = Stats;

        var inventory = World.Instance.CurrentPlayer.Inventory;
        Stats.Items = inventory.Items;
        Stats.HotbarToItemIndexMap = inventory.HotbarToItemIndexMap;
    }
}
