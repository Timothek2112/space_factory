using Godot;
using System;
using System.Collections.Generic;

public interface IPutable
{
	public List<Slot> inputSlots { get; set; }
	public List<Slot> outputSlots { get; set; }
	public int PutItem(Items type, int count);
	public void RemoveItem(Items type, int count);
	public bool CanAcceptItem(Items type, int count);
	public bool CanGiveItem(Items type, int count);
}
