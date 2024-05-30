using Godot;
using GodotUtilities;
using GodotUtilities.SourceGenerators;

namespace SupaLidlGame.UI.Inventory;

public partial class InventorySlot : Container
{
    private bool _isSelected = false;

    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            _isSelected = value;
            if (_selectedFrame is not null)
            {
                _selectedFrame.Visible = _isSelected;
                _frame.Visible = !_isSelected;
            }
        }
    }

    private TextureRect _textureRect;

    private NinePatchRect _frame;

    private NinePatchRect _selectedFrame;

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

    public override void _Ready()
    {
        _textureRect = GetNode<TextureRect>("TextureRect");
        _frame = GetNode<NinePatchRect>("Frame");
        _selectedFrame = GetNode<NinePatchRect>("SelectedFrame");
    }
}
