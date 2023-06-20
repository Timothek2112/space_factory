using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using static Godot.GD;

public partial class StorageInventoryUI : NinePatchRect, IInventory
{
    public int slotsCount;
    [Export] PackedScene itemSlot;
    [Export] PackedScene slotBackground;
    PlayerController playerController;
    public IPutable origin { get; set; }

    public override void _Ready()
    {
        base._Ready();
        playerController = GetNode<PlayerController>("/root/PlayerController");
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        UpdateSlots();
    }

    public void UpdateSlots()
    {
        var children = GetNode<Control>("Container").GetChildren().ToList();
        foreach (var slot in origin.inputSlots)
        {
            var thisSlot = (ItemUI)children.FirstOrDefault(p => ((ItemUI)p).slot == slot);
            if (thisSlot == null)
            {
                continue;
            }
            thisSlot.count = slot.currentCount.ToString();
        }
        if (origin.inputSlots.Count != children.Count)
        {
            ShowInventory();
        }
    }

    public void ShowInventory()
    {
        var container = GetNode<Control>("Container");
        var slotContainer = GetNode<Control>("SlotBackground");
        foreach (var child in container.GetChildren())
        {
            container.RemoveChild(child);
        }
        foreach (var slot in slotContainer.GetChildren())
        {
            slotContainer.RemoveChild(slot);
        }
        foreach (var slot in origin.inputSlots)
        {
            var instance = itemSlot.Instantiate();
            var itemUI = (ItemUI)instance;
            itemUI.SetSprite(GD.Load<Texture2D>("res://Icons/water_icon_inventory.png"));
            itemUI.SetCount(slot.currentCount);
            itemUI.slot = slot;
            container.AddChild(instance);
        }
        for (int i = 0; i < slotsCount; i++)
        {
            var instanceBackground = slotBackground.Instantiate();
            slotContainer.AddChild(instanceBackground);
        }
    }

    public void _on_button_button_down()
    {
        this.QueueFree();
    }

    public void _on_mouse_entered()
    {
        playerController.inventoryInFocus = this;
    }

    public void _on_mouse_exited()
    {
        playerController.inventoryInFocus = null;
    }
}
