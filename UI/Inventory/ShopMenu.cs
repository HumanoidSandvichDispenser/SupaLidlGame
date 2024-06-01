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
            GD.Print("Set ShopMenu source");
            _source = value;
            _inventoryGrid.Source = value;
        }
    }

    [Export]
    private InventoryGrid _inventoryGrid;

    private InventorySlot _selected;

    public void HideModal()
    {
        Hide();
        _source = null;
    }

    public override void _Ready()
    {
        Events.EventBus.Instance.EnterShop += (string path) =>
        {
            var shop = ResourceLoader.Load<Items.Shop>(path);
            GD.Print("Loaded shop");
            Source = shop;
        };

        _inventoryGrid.SlotFocused += (InventorySlot slot) =>
        {
            GD.Print("SlotFocused " + slot.Name);
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

        GetNode<Button>("%BuyButton").GuiInput += (Godot.InputEvent @event) =>
        {
            if (@event.IsAction("ui_cancel"))
            {
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
}
