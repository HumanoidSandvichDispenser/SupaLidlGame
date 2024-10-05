using Godot;
using GodotUtilities;
using GodotUtilities.SourceGenerators;

namespace SupaLidlGame.UI.Inventory;

public partial class InventorySlot : Button
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
                //_selectedFrame.Visible = _isSelected;
                //_frame.Visible = !_isSelected;
            }
        }
    }

    [Export]
    public bool UseFocusAsSelected { get; set; } = true;

    private TextureRect _textureRect;

    private NinePatchRect _frame;

    private NinePatchRect _selectedFrame;

    private static Texture2D _placeholderTexture;

    public int Index { get; set; }

    private Items.ItemMetadata _item;

    public Items.ItemMetadata Item
    {
        get => _item;
        set
        {
            _item = value;

            if (_item is null)
            {
                //_textureRect.Texture = null;
                Icon = null;
            }
            else
            {
                //_textureRect.Texture = _item.Icon;
                Icon = _item.Icon;
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

        if (Item is null)
        {
            // do this to reset the icon
            Item = null;
        }

        if (UseFocusAsSelected)
        {
            void focusEntered()
            {
                IsSelected = true;
            }

            void focusExited()
            {
                IsSelected = false;
            }

            Connect(SignalName.FocusEntered, Callable.From(focusEntered));
            Connect(SignalName.FocusExited, Callable.From(focusExited));

            Connect(SignalName.MouseEntered, Callable.From(focusEntered));
            Connect(SignalName.MouseExited, Callable.From(focusExited));
        }
    }
}
