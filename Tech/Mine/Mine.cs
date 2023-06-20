using Godot;
using static Godot.GD;
using System;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Newtonsoft.Json;

public partial class Mine : BaseBuilding
{
	private bool producting;
	public List<Item> miningResources = new List<Item>();
	public float itemsPerSecond;

	public override void _Ready()
	{	
		base._Ready();
		var json = (Json)GD.Load("res://Tech/Config/Mines.tres");
		var mines = JsonConvert.DeserializeObject<List<Mine>>(json.Data.ToString());
		var self = mines.FirstOrDefault(p => p.id == id);
		miningResources = new List<Item>(self.miningResources);
		itemsPerSecond = self.itemsPerSecond;
		inputSlots = new List<Slot>(self.inputSlots);
		outputSlots = new List<Slot>(self.outputSlots);
		name = self.name;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if(!producting)
			Production();
	}

	private async Task Production(){
		producting = true;
		await ToSignal(GetTree().CreateTimer(1/itemsPerSecond), SceneTreeTimer.SignalName.Timeout);
		CreateItems();
		producting = false;
	}

	private void CreateItems(){
		foreach(var item in miningResources){
			var storageForResource = outputSlots.FirstOrDefault(p => p.type == item.type);
			if(storageForResource == null)
				return;
			//Print(storageForResource.type + ": " + storageForResource.currentCount + "/" + storageForResource.capability);
			if(storageForResource.capability > storageForResource.currentCount)
				storageForResource.currentCount += 1;
		}
	}

	public override void ShowInventory()
	{
		throw new NotImplementedException();
    }

	public override int PutItem(Items type, int count)
    {
		throw new NotImplementedException();
    }

	public override void RemoveItem(Items type, int count)
    {
		var slotOfItem = outputSlots.FirstOrDefault(p => p.type == type && p.currentCount > count);
		if(slotOfItem == null)
			return;
		slotOfItem.currentCount -= count;
    }
	
	public override bool CanGiveItem(Items type, int count){
		var slotOfItem = outputSlots.FirstOrDefault(p => (p.type == type || type == Items.any) && p.currentCount > count);
		if(slotOfItem == null)
			return false;
		return true;
	}
}
