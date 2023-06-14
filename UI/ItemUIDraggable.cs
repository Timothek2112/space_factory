using Godot;
using System;

public partial class ItemUIDraggable : Control
{
	PlayerController playerController;

	public override void _Ready()
	{
		base._Ready();
		playerController = GetNode<PlayerController>("/root/PlayerController");
	}

	public override void _Process(double delta)
	{
		this.GlobalPosition = GetGlobalMousePosition();
		if (Input.IsActionJustReleased("Click"))
		{
			playerController.EndDraggingItem();
			this.QueueFree();
		}
	}
}
