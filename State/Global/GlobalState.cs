using Godot;
using SupaLidlGame.Utils;

namespace SupaLidlGame.State.Global;

public partial class GlobalState : Node
{
    [Export]
    public Progression Progression { get; set; } = new();

    [Export]
    public MapState MapState { get; set; } = new();

    [Export]
    public Stats Stats { get; set; } = new();

    [Export]
    public GameSettings Settings { get; set; } = new();

    [Signal]
    public delegate void SummonBossEventHandler(string bossName);

    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Always;
        LoadSettings();
    }

    public override void _Notification(int what)
    {
        if (what == NotificationWMCloseRequest)
        {
            // TODO: quit prompt
            GetTree().Root
                .PropagateNotification((int)NotificationWMCloseRequest);
            SaveSettings();
        }
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
    }

    public void ExportToSave(Save save)
    {
        save.Progression = Progression;
        save.MapState = MapState;
        save.Stats = Stats;
    }
}
