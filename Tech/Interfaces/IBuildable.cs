using Godot;
using System;

public interface IBuildable
{
	public bool built {get; set;}
	public void Build();
	public void ShowBlueprint();
}
