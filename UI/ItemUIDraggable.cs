using Godot;
using System;

public partial class ItemUIDraggable : Control
{
	PlayerController playerController;
	public ItemUI itemUI;
	public Label countNode;


    public override void _Ready()
	{
		base._Ready();
		playerController = GetNode<PlayerController>("/root/PlayerController");
		countNode = GetNode<Label>("Count");
    }

    public override void _Process(double delta)
	{
		this.GlobalPosition = GetGlobalMousePosition();
		countNode.Text = itemUI.count.ToString();
		if (Input.IsActionJustReleased("Click"))
		{
			TryPutItem();
			playerController.EndDraggingItem();
			this.QueueFree();
		}
	}

	public bool TryPutItem(){
		if(!playerController.isInventoryInFocus)
			return false;

		var transferedItems = playerController.inventoryInFocus.origin.PutItem(itemUI.slot.type, int.Parse(itemUI.count));
		itemUI.slot.currentCount -= transferedItems;
		return true;
	}
}
