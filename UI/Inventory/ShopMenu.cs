using Godot;
using System.Collections.Generic;

namespace SupaLidlGame.UI.Inventory;

public partial class ShopMenu : BaseMenu, IModal
{
    private Items.IItemCollection<Items.ShopEntry> _source;

    public Items.IItemCollection<Items.ShopEntry> Source
    {
        get => _source;
        set
        {
            _source = value;
            _inventoryGrid.Source = value;
        }
    }

    public override void ShowModal()
    {
        Show();
        var animPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
        animPlayer.Play("open");
    }

    public override void HideModal()
    {
        Hide();
        _source = null;
    }

    public override void _Ready()
    {
        base._Ready();

        _focusButtonOnSelect = GetNode<Button>("%BuyButton");

        GetNode<Button>("%BuyButton").GuiInput += (InputEvent @event) =>
        {
            if (@event.IsActionPressed("ui_cancel"))
            {
                GetViewport().SetInputAsHandled();
                _selected?.GrabFocus();
            }
        };
    }

    protected override void SetTooltipItem(InventorySlot slot)
    {
        base.SetTooltipItem(slot);

        if (slot == _selected)
        {
            GetNode<Button>("%BuyButton").Disabled = false;
        }
        else
        {
            GetNode<Button>("%BuyButton").Disabled = true;
        }
    }

    public override void OnButtonPress(Button button)
    {
        throw new System.NotImplementedException("Not yet implemented.");
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
        {
            GetViewport().SetInputAsHandled();
            Close();
        }
    }
}
