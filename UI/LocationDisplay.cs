using Godot;
using SupaLidlGame.Extensions;

namespace SupaLidlGame.UI;

public partial class LocationDisplay : CenterContainer
{
    private Label _mapName;
    private Label _subtitle;
    private AnimationPlayer _animPlayer;

    public override void _Ready()
    {
        _mapName = GetNode<Label>("%MapName");
        _subtitle = GetNode<Label>("%Subtitle");
        _animPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");

        var bus = this.GetEventBus();
        bus.AreaChanged += OnAreaChanged;
    }

    public void OnAreaChanged(Scenes.Map map)
    {
        //var map = args.Map;

        // if the area name is the same as the map name, do not show a subtitle
        _subtitle.Visible = map.AreaName != map.MapName;
        _mapName.Text = map.MapName;
        _subtitle.Text = map.AreaName;
        _animPlayer.Play("show");
    }
}
