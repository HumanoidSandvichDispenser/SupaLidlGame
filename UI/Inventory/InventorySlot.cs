using Godot;
using GodotUtilities;
using GodotUtilities.SourceGenerators;

namespace SupaLidlGame.UI.Inventory;

[Scene]
public partial class InventorySlot : ColorRect
{
    [Node("TextureRect")]
    protected TextureRect _textureRect;

    [Node("Selected")]
    protected NinePatchRect _frame;

    protected static Texture2D _placeholderTexture;

    private Items.ItemMetadata _item;

    public Items.ItemMetadata Item
    {
        get => _item;
        set
        {
            _item = value;

            if (_item is null)
            {
                _textureRect.Texture = null;
            }
            else
            {
                _textureRect.Texture = _item.Icon;
            }
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
}
