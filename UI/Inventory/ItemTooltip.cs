using Godot;
using SupaLidlGame.Items;

namespace SupaLidlGame.UI.Inventory;

public partial class ItemTooltip : Control
{
    private ItemMetadata _item;

    public ItemMetadata Item
    {
        get => _item;
        set
        {
            _item = value;
            if (IsNodeReady())
            {
                Render();
            }
        }
    }

    public override void _Ready()
    {
        Render();
    }

    public void Render()
    {
        if (_item is null)
        {
            GetNode<Label>("%ItemLabel").Text = "Nothing here";
            GetNode<TextureRect>("%ItemTexture").Texture = null;
            GetNode<RichTextLabel>("%ItemDescription").Text = "We are ready for My Summer Car";
            GetNode<Label>("%Ingredients/Label").Text = "0 Shillings";
            return;
        }

        GetNode<Label>("%ItemLabel").Text = _item.Name;
        GetNode<TextureRect>("%ItemTexture").Texture = _item.Icon;
        GetNode<RichTextLabel>("%ItemDescription").Text = _item.Description;
        GetNode<Label>("%Ingredients/Label").Text = _item.BuyPrice + " Shillings";
    }
}
