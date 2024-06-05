using Godot;
using GodotUtilities;
using SupaLidlGame.Items;

namespace SupaLidlGame.UI.Inventory;

public partial class InventoryGrid : GridContainer
{
    private SupaLidlGame.Items.IItemCollection _source;

    [Export]
    private PackedScene _slotScene;

    public ButtonGroup ButtonGroup { get; private set; }

    public SupaLidlGame.Items.IItemCollection Source
    {
        get => _source;
        set
        {
            GD.Print("Set InventoryGrid source");
            _source = value;
            Redraw();
        }
    }

    [Signal]
    public delegate void SlotFocusedEventHandler(InventorySlot slot);

    [Signal]
    public delegate void SlotUnfocusedEventHandler(InventorySlot slot);

    [Signal]
    public delegate void SlotSelectedEventHandler(InventorySlot slot);

    public InventoryGrid()
    {
        ButtonGroup = new();
    }

    public void Redraw()
    {
        GD.Print("Redrawing inventory grid...");

        if (_source is null)
        {
            this.QueueFreeChildren();
        }

        var children = GetChildren();

        for (int i = children.Count; i < _source.Capacity; i++)
        {
            AddInventorySlot();
        }

        for (int i = children.Count - 1; i >= _source.Capacity; i--)
        {
            children[i].QueueFree();
        }

        children = GetChildren();

        // iterate through items and update the grid
        using (var items = _source.GetItems().GetEnumerator())
        {
            GD.Print("Updating items...");
            int i;
            for (i = 0; items.MoveNext(); i++)
            {
                InventorySlot slot = children[i] as InventorySlot;

                ItemMetadata item = items.Current;
                slot.Item = item;
            }

            // make remaining slots display empty
            for (; i < _source.Capacity; i++)
            {
                InventorySlot slot = children[i] as InventorySlot;

                slot.Item = null;
            }
        }

        for (int i = 0; i < children.Count; i++)
        {
            var child = children[i] as Control;
            if (i > 0)
            {
                child.FocusPrevious = child.GetPathTo(children[i - 1]);
            }
            if (i < children.Count - 1)
            {
                child.FocusNext = child.GetPathTo(children[i + 1]);
            }
        }

        if (children.Count > 0)
        {
            var button = children[0] as Button;
            button.ButtonPressed = true;
            button.GrabFocus();
        }
    }

    private InventorySlot AddInventorySlot()
    {
        var slot = _slotScene.Instantiate<InventorySlot>();
        AddChild(slot);
        slot.ButtonGroup = ButtonGroup;

        void focusedHandler()
        {
            EmitSignal(SignalName.SlotFocused, slot);
        }

        void unfocusedHandler()
        {
            EmitSignal(SignalName.SlotUnfocused, slot);
        }

        void toggledHandler()
        {
            EmitSignal(SignalName.SlotSelected, slot);
        }

        slot.Connect(
            InventorySlot.SignalName.FocusEntered,
            Callable.From(focusedHandler)
        );

        slot.Connect(
            InventorySlot.SignalName.FocusExited,
            Callable.From(unfocusedHandler)
        );

        slot.Connect(
            InventorySlot.SignalName.MouseEntered,
            Callable.From(focusedHandler)
        );

        slot.Connect(
            InventorySlot.SignalName.MouseExited,
            Callable.From(unfocusedHandler)
        );

        slot.Connect(
            InventorySlot.SignalName.Pressed,
            Callable.From(toggledHandler)
        );

        return slot;
    }

    private void RemoveInventorySlot(InventorySlot slot)
    {
        RemoveChild(slot);
    }

    public bool GrabSlotFocus()
    {
        var children = GetChildren();
        if (children.Count > 0)
        {
            var button = children[0] as Button;
            button.GrabFocus();
            return true;
        }
        return false;
    }
}
