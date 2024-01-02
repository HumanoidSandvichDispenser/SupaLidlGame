using Godot;
using GodotUtilities;
using GodotUtilities.SourceGenerators;

namespace SupaLidlGame.UI;

[Scene]
public partial class InventorySlot : ColorRect
{
    [Node("TextureRect")]
    private TextureRect _textureRect;

    [Node("Selected")]
    private NinePatchRect _selected;

    [Node("Unselected")]
    private NinePatchRect _unselected;

    private static Texture2D _placeholderTexture;

    private Items.ItemMetadata _item;

    public Items.ItemMetadata Item
    {
        get => _item;
        set
        {
            _item = value;

            if (_item is null)
            {
                //_textureRect.Texture = _placeholderTexture;
                _textureRect.Texture = null;
            }
            else
            {
                _textureRect.Texture = _item.Icon;
            }
        }
    }

    private bool _isSelected = false;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            _selected.Visible = _isSelected;
            _unselected.Visible = !_isSelected;
        }
    }

    static InventorySlot()
    {
        _placeholderTexture = ResourceLoader.Load<Texture2D>(
            "res://Assets/Sprites/UI/hotbar-inactive.png");
    }

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            WireNodes();
        }
        base._Notification(what);
    }

    public override void _Ready()
    {

    }
}
