using Godot;
using System;

public partial class Slot
{
	public string uuid = Guid.NewGuid().ToString();
	public int capability;
	public int currentCount;
	public Items type;

	public Slot(Items type, int capability){
		this.uuid = Guid.NewGuid().ToString();
		this.capability = capability;
		this.type = type;
	}
}
