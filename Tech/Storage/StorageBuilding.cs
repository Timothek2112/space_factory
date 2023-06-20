using static Godot.GD;
using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

public partial class StorageBuilding : BaseBuilding
{
	[Export]
	public int slots;
	public int slotSize;

	public override void _Ready()
	{
		base._Ready();
		var json = (Json)GD.Load("res://Tech/Config/Storages.tres");
		var storages = JsonConvert.DeserializeObject<List<StorageBuilding>>(json.Data.ToString());
		var self = storages.FirstOrDefault(p => p.id == id);
		inputSlots = new List<Slot>(self.inputSlots);
		outputSlots = new List<Slot>(self.outputSlots);
		name = self.name;
		slots = self.slots;
		slotSize = self.slotSize;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	public override int PutItem(Items type, int count){
		var slot = inputSlots.FirstOrDefault(p => p.type == type && p.currentCount + count <= p.capability);
		if(slot == null){
			if(inputSlots.Count >= slots)
				return 0;
			slot = new Slot(type, slotSize);
			inputSlots.Add(slot);
		}
		slot.currentCount += count;
		return count;
	}

	public override bool CanAcceptItem(Items type, int count){
		var slot = inputSlots.FirstOrDefault(p => p.type == type && p.currentCount < p.capability);
		if(slot == null && slots <= inputSlots.Count)
			return false;
		else
			return true;
	}

	public override void ShowInventory()
	{
		var instance = (StorageInventoryUI)inventoryScene.Instantiate();
		GetTree().GetFirstNodeInGroup("MainCanvas").AddChild(instance);
		instance.origin = this;
		instance.slotsCount = slots;
		instance.ShowInventory();
	}
}
