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
		var mines = JsonConvert.DeserializeObject<List<StorageBuilding>>(json.Data.ToString());
		var self = mines.FirstOrDefault(p => p.id == id);
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

	public override void PutItem(PipeNode from){
		
		var slot = inputSlots.FirstOrDefault(p => p.type == from.itemsType && p.currentCount < p.capability);
		if(slot == null){
			slot = new Slot(from.itemsType, slotSize);
			inputSlots.Add(slot);
		}
		slot.currentCount += 1;
		from.itemsInPipe -= 1;
		foreach(var islot in inputSlots){
			Print(islot.currentCount);
			Print(islot.type);
			Print();
		}
		Print();
	}

	public override bool CanAcceptItem(Items type, int count){
		var slot = inputSlots.FirstOrDefault(p => p.type == type && p.currentCount < p.capability);
		if(slot == null && slots <= inputSlots.Count)
			return false;
		else
			return true;
	}	
}
