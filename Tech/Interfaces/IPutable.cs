using Godot;
using System;

public interface IPutable
{
	public void PutItem(PipeNode from);
	public void GiveItem(PipeNode to);
	public bool CanAcceptItem(Items type, int count);
}
