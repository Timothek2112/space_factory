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
	public void SetSprite(Texture2D image)
	{
		GetNode<TextureRect>("Sprite").Texture = image;
	}

	public void SetCount(int count)
	{
		GetNode<Label>("Count").Text = count.ToString();
	}
}
