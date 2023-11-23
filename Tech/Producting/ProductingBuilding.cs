using Godot;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class ProductingBuilding : BaseBuilding
{
    public List<Item> productingResources = new List<Item>();
    public float itemsPerSecond;
    private bool producting;
    public List<Item> requiresToProduct = new List<Item>();

    public override void _Ready()
	{
		base._Ready();
        var json = (Json)GD.Load("res://Tech/Config/Producting.tres");
        var producting = JsonConvert.DeserializeObject<List<ProductingBuilding>>(json.Data.ToString());
        var self = producting.FirstOrDefault(p => p.id == id);
        inputSlots = new List<Slot>(self.inputSlots);
        outputSlots = new List<Slot>(self.outputSlots);
        requiresToProduct = new List<Item>(self.requiresToProduct);
        productingResources = new List<Item>(self.productingResources);
        itemsPerSecond = self.itemsPerSecond;
        name = self.name;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (!producting && CanProduct())
            Production();
    }

    private async Task Production()
    {
        SubstractResourcesForProduct();
        producting = true;
        await ToSignal(GetTree().CreateTimer(1 / itemsPerSecond), SceneTreeTimer.SignalName.Timeout);
        CreateItems();
        producting = false;
    }

    public void CreateItems()
    {
        foreach(var item in productingResources)
        {
            var slotForRes = outputSlots.FirstOrDefault(p => p.type == item.type && p.currentCount + item.count <= p.capability);
            if (slotForRes == null)
                return;
            slotForRes.currentCount += item.count;
        }
    }

    public void SubstractResourcesForProduct()
    {
        foreach(var item in requiresToProduct)
        {
            var slot = inputSlots.FirstOrDefault(p => p.type == item.type && p.currentCount >= item.count);
            slot.currentCount -= item.count;
        }
    }

    public bool CanProduct()
    {
        foreach(var item in requiresToProduct)
        {
            if (!IsHaveInputItems(item.type, item.count))
                return false;
        }
        return true;
    }

    public bool IsHaveInputItems(Items type, int count)
    {
        var itemSlot = inputSlots.FirstOrDefault(p => p.type == type && p.currentCount >= count);
        return itemSlot != null;
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
