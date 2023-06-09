using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class StorageInventory : NinePatchRect
{
	public List<Slot> slots;
	[Export] PackedScene itemSlot;

    public override void _Process(double delta)
    {
        base._Process(delta);
		foreach(var slot in slots)
		{
			var children = GetNode<Control>("Container").GetChildren().ToList();
			var thisSlot = (ItemUI)children.FirstOrDefault(p => ((ItemUI)p).uuid == slot.uuid);
            if (thisSlot == null)
			{
				continue;
			}
			
			thisSlot.count = slot.currentCount.ToString();

			if(slots.Count != children.Count)
			{
				ShowStoreInventory();
			}
        }
    }

    public void SetSlots(List<Slot> slots)
	{
		this.slots = slots;
	}

	public void ShowStoreInventory()
	{
		var container = GetNode<Control>("Container");
		foreach(var child in container.GetChildren())
		{
			container.RemoveChild(child);
		}
		foreach(var slot in slots)
		{
			var instance = itemSlot.Instantiate();
			var itemUI = (ItemUI)instance;
            itemUI.SetSprite(GD.Load<Texture2D>("res://Icons/water_icon_inventory.png"));
            itemUI.SetCount(slot.currentCount);
			itemUI.uuid = slot.uuid;
			container.AddChild(instance);
		}
	}

	public void _on_button_button_down()
	{
		this.QueueFree();
	}
}
