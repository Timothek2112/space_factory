using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ProductingInventoryUI : Control, IInventory
{
    [Export] PackedScene itemSlot;
    [Export] PackedScene slotBackground;
    PlayerController playerController;
    public IPutable origin { get; set; }
    public int inputSlotsCount;
    public int outputSlotsCount;

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

    public void ShowInventory()
    {
        var input = GetNode<Control>("Input");
        var output = GetNode<Control>("Output");
        var inputBackground = GetNode<Control>("InputSlots");
        var outputBackground = GetNode<Control>("OutputSlots");

        ClearChilds(input);
        ClearChilds(inputBackground);
        ClearChilds(output);
        ClearChilds(outputBackground);

        foreach (var slot in origin.inputSlots)
        {
            var instance = itemSlot.Instantiate();
            var itemUI = (ItemUI)instance;
            itemUI.SetSprite(GD.Load<Texture2D>("res://Icons/Items/water_barrel_32.png"));
            itemUI.SetCount(slot.currentCount);
            itemUI.slot = slot;
            input.AddChild(instance);
        }
        foreach (var slot in origin.outputSlots)
        {
            var instance = itemSlot.Instantiate();
            var itemUI = (ItemUI)instance;
            itemUI.SetSprite(GD.Load<Texture2D>("res://Icons/Items/water_barrel_32.png"));
            itemUI.SetCount(slot.currentCount);
            itemUI.slot = slot;
            output.AddChild(instance);
        }
        for (int i = 0; i < inputSlotsCount; i++)
        {
            var instanceBackground = slotBackground.Instantiate();
            inputBackground.AddChild(instanceBackground);
        }
        for (int i = 0; i < outputSlotsCount; i++)
        {
            var instanceBackground = slotBackground.Instantiate();
            outputBackground.AddChild(instanceBackground);
        }
    }

    public void ClearChilds(Control toClear)
    {
        foreach (var child in toClear.GetChildren())
        {
            toClear.RemoveChild(child);
        }
    }

    public void UpdateSlots()
    {
        var input = GetNode<Control>("Input").GetChildren().ToList();
        var output = GetNode<Control>("Output").GetChildren().ToList();
        foreach (var slot in origin.inputSlots)
        {
            var thisSlot = (ItemUI)input.FirstOrDefault(p => ((ItemUI)p).slot == slot);
            if (thisSlot == null)
            {
                continue;
            }
            thisSlot.count = slot.currentCount.ToString();
        }
        foreach (var slot in origin.outputSlots)
        {
            var thisSlot = (ItemUI)output.FirstOrDefault(p => ((ItemUI)p).slot == slot);
            if (thisSlot == null)
            {
                continue;
            }
            thisSlot.count = slot.currentCount.ToString();
        }
        if (origin.inputSlots.Count != input.Count || origin.outputSlots.Count != output.Count)
        {
            ShowInventory();
        }
    }
}
