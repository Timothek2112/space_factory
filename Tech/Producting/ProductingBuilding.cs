using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ProductingBuilding : BaseBuilding
{
    public List<Item> productingResources = new List<Item>();
    public float itemsPerSecond;
    private bool producting;

    public override void _Ready()
	{
		base._Ready();
        var json = (Json)GD.Load("res://Tech/Config/Producting.tres");
        var producting = JsonConvert.DeserializeObject<List<ProductingBuilding>>(json.Data.ToString());
        var self = producting.FirstOrDefault(p => p.id == id);
        inputSlots = new List<Slot>(self.inputSlots);
        outputSlots = new List<Slot>(self.outputSlots);
        productingResources = new List<Item>(self.productingResources);
        itemsPerSecond = self.itemsPerSecond;
        name = self.name;
    }

    public override void ShowInventory()
    {
        var instance = (ProductingInventoryUI)inventoryScene.Instantiate();
        instance.origin = this;
        instance.inputSlotsCount = inputSlots.Count;
        instance.outputSlotsCount = outputSlots.Count;
        GetTree().GetFirstNodeInGroup("MainCanvas").AddChild(instance);
        instance.ShowInventory();
    }

    public override void _Process(double delta)
	{
		base._Process(delta);
	}

    public override void RemoveItem(Items type, int count)
    {
        var slotOfItem = outputSlots.FirstOrDefault(p => p.type == type && p.currentCount >= count);
        if (slotOfItem == null)
            return;
        slotOfItem.currentCount -= count;
    }

    public override bool CanGiveItem(Items type, int count)
    {
        var slotOfItem = outputSlots.FirstOrDefault(p => (p.type == type || type == Items.any) && p.currentCount >= count);
        if (slotOfItem == null)
            return false;
        return true;
    }
}
