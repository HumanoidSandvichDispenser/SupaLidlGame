using Godot;
using System.Collections.Generic;

namespace SupaLidlGame.UI.Inventory;

public partial class ShopMenu : Control, IModal
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

    [Export]
    private InventoryGrid _inventoryGrid;

    private InventorySlot _selected;

    public void ShowModal()
    {
        Show();
        var animPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
        animPlayer.Play("open");
    }

    public void HideModal()
    {
        Hide();
        _source = null;
    }

    public async void Close()
    {
        var animPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
        animPlayer.Play("close");
        await ToSignal(animPlayer, AnimationPlayer.SignalName.AnimationFinished);
        HideModal();
    }

    public override void _Ready()
    {
        _inventoryGrid.SlotFocused += (InventorySlot slot) =>
        {
            if (slot.Item is not null)
            {
                SetTooltipItem(slot);
            }
        };

        _inventoryGrid.SlotUnfocused += (InventorySlot slot) =>
        {
            SetTooltipItem(_selected);
        };

        _inventoryGrid.SlotSelected += (InventorySlot slot) =>
        {
            _selected = slot;
            SetTooltipItem(slot);
            GetNode<Button>("%BuyButton").GrabFocus();
        };

        GetNode<Button>("%BuyButton").GuiInput += (InputEvent @event) =>
        {
            if (@event.IsActionPressed("ui_cancel"))
            {
                GetViewport().SetInputAsHandled();
                _selected?.GrabFocus();
            }
        };
    }

    private void SetTooltipItem(InventorySlot slot)
    {
        GetNode<ItemTooltip>("%ItemTooltip").Item = slot?.Item;

        if (slot == _selected)
        {
            GetNode<Button>("%BuyButton").Disabled = false;
        }
        else
        {
            GetNode<Button>("%BuyButton").Disabled = true;
        }
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
