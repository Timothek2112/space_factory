using Godot;
using System;
using System.Collections.Generic;

public interface IPutable
{
	public List<Slot> inputSlots { get; set; }
	public List<Slot> outputSlots { get; set; }
	public void PutItem(PipeNode from);
	public void GiveItem(PipeNode to);
	public bool CanAcceptItem(Items type, int count);
}
