using Godot;
using System;

public partial class ItemUI : Control
{
	public string uuid;
	public string count
	{
		get
		{
			return GetNode<Label>("Count").Text;
        }
		set
		{
			GetNode<Label>("Count").Text = value;
        }
	}
	public PlayerController playerController;
	[Export] PackedScene itemDraggable;

    public override void _Ready()
    {
        base._Ready();
		playerController = GetNode<PlayerController>("/root/PlayerController");
    }

    public void SetSprite(Texture2D image)
	{
		GetNode<TextureRect>("Sprite").Texture = image;
	}

	public void SetCount(int count)
	{
		GetNode<Label>("Count").Text = count.ToString();
	}

	public void _on_sprite_gui_input(InputEvent @event)
	{
        if (Input.IsActionJustPressed("Click"))
        {
            if (!playerController.CanStartDragItem())
                return;
            var instance = itemDraggable.Instantiate();
            playerController.StartDraggingItem((ItemUIDraggable)instance);
            GetTree().GetFirstNodeInGroup("MainUI").AddChild(instance);

        }
    }
}
