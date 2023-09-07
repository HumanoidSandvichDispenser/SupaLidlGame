using Godot;
using SupaLidlGame.Events;

namespace SupaLidlGame.UI;

public partial class LevelBar : Control
{
    public TextureProgressBar XPBar { get; set; }

    private TextureProgressBar[] _levelBars = new TextureProgressBar[4];

    public override void _Ready()
    {
        XPBar = GetNode<TextureProgressBar>("%XPBar");

        for (int i = 0; i < 4; i++)
        {
            _levelBars[i] = GetNode<TextureProgressBar>($"%Level{i + 1}Bar");
        }

        EventBus.Instance.PlayerXPChanged += (xp) =>
        {
            XPBar.Value = xp;
        };

        EventBus.Instance.PlayerLevelChanged += (level) =>
        {
            for (int i = 0; i < _levelBars.Length; i++)
            {
                // level 0: 0 is not less than 0
                // level 1: 0 is less than 1
                _levelBars[i].Value = i < level ? 1 : 0;
            }
        };
    }
}
