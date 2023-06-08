using Godot;
using System;

public partial class DraggableUI : TextureRect
{
	[Export]
	bool isMouseHovering;
	Vector2 offset = Vector2.Zero;
	public override void _Input(InputEvent @event){
		if(!isMouseHovering)
			return;

		if(Input.IsActionJustPressed("Click")){
			offset = -GetGlobalMousePosition() + this.GetParent<Control>().GlobalPosition;
		}
		
		if(Input.IsActionPressed("Click")){
			this.GetParent<Control>().GlobalPosition = GetGlobalMousePosition() + offset;
		}
	}

	private void _on_mouse_entered(){
		isMouseHovering = true;
	}

	private void _on_mouse_exited(){
		isMouseHovering = false;
	}
}
