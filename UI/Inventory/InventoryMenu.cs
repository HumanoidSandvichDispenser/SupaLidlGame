using Godot;
using System.Collections.Generic;

namespace SupaLidlGame.UI.Inventory;

public partial class InventoryMenu : BaseMenu, IModal
{
    private Items.Inventory _source;

    public Items.Inventory Source
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

        _focusButtonOnSelect = GetNode<Button>("%SlotButton1");

        foreach (Node node in GetTree().GetNodesInGroup("SlotButtons"))
        {
            var button = node as Button;

            var onButtonPress = () =>
            {
                OnButtonPress(button);
            };

            var onButtonInput = (InputEvent @event) =>
            {
                if (@event.IsActionPressed("ui_cancel"))
                {
                    GetViewport().SetInputAsHandled();
                    _selected?.GrabFocus();
                }
            };

            button.Connect(Button.SignalName.Pressed, Callable.From(onButtonPress));
        }
    }

    protected override void SetTooltipItem(InventorySlot slot)
    {
        base.SetTooltipItem(slot);

        if (slot == _selected)
        {
            GetTree().SetGroup("SlotButtons", "disabled", false);
        }
        else
        {
            GetTree().SetGroup("SlotButtons", "disabled", true);
        }
    }

    public override void OnButtonPress(Button button)
    {
        int slot = button.GetMeta("slot").AsInt32();
        GD.Print("Equipping item at slot " + slot);
        Source.SetHotbarIndexToItem(slot, _selected.Item);
    }
}
