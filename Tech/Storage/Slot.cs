using Godot;
using System;

public partial class Slot
{
	public int capability;
	public int currentCount;
	public Items type;

	public Slot(Items type, int capability){
		this.capability = capability;
		this.type = type;
	}
}
