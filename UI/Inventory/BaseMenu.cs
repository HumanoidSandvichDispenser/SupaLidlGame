using Godot;

namespace SupaLidlGame.UI.Inventory;

public abstract partial class BaseMenu : Control, IModal
{
    [Export]
    protected InventoryGrid _inventoryGrid;

    protected Button _focusButtonOnSelect { get; set; }

    protected InventorySlot _selected;

    public abstract void ShowModal();

    public abstract void HideModal();

    public bool IsPlayingAnimation => GetNode<AnimationPlayer>("%AnimationPlayer").IsPlaying();

    public async void Close()
    {
        var animPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
        animPlayer.Play("close");
        await ToSignal(animPlayer, AnimationPlayer.SignalName.AnimationFinished);
        HideModal();
    }

    public override void _Ready()
    {
        var onSlotFocused = (InventorySlot slot) =>
        {
            if (slot.Item is not null)
            {
                SetTooltipItem(slot);
            }
        };

        var onSlotUnfocused = (InventorySlot slot) =>
        {
            SetTooltipItem(_selected);
        };

        var onSlotSelected = (InventorySlot slot) =>
        {
            _selected = slot;
            SetTooltipItem(slot);
            //GetNode<ItemTooltip>("%ActionButton").GrabFocus();
            _focusButtonOnSelect.GrabFocus();
        };

        _inventoryGrid.Connect(InventoryGrid.SignalName.SlotFocused,
            Callable.From(onSlotFocused));
        _inventoryGrid.Connect(InventoryGrid.SignalName.SlotUnfocused,
            Callable.From(onSlotUnfocused));
        _inventoryGrid.Connect(InventoryGrid.SignalName.SlotSelected,
            Callable.From(onSlotSelected));
    }

    protected virtual void SetTooltipItem(InventorySlot slot)
    {
        GetNode<ItemTooltip>("%ItemTooltip").Item = slot?.Item;
    }

    public abstract void OnButtonPress(Button button);

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsActionPressed("ui_cancel"))
        {
            AcceptEvent();
            Close();
        }
    }
}
