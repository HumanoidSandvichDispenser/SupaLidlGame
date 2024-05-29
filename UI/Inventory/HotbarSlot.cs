using Godot;
using GodotUtilities;
using GodotUtilities.SourceGenerators;

namespace SupaLidlGame.UI.Inventory;

[Scene]
public partial class HotbarSlot : InventorySlot
{
    [Node("TextureRect")]
    private TextureRect _textureRect;

    [Node("Selected")]
    private NinePatchRect _selected;

    private static Texture2D _placeholderTexture;

    private Items.ItemMetadata _item;

    private bool _isSelected = false;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            _selected.Visible = _isSelected;
            _frame.Visible = !_isSelected;
        }
    }
}
