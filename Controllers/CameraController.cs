using static Godot.GD;
using Godot;
using System;

public partial class CameraController : Camera2D
{
	bool isGrounded = false;
	public Planet groundedTo;
	[Export]
	float groundedZoom = 1.5f;
	[Export]
	float normalZoom = 1;

	public override void _Ready()
	{
		GetNode<CosmosController>("/root/CosmosController").PlayerGroundedChanged += OnPlayerGroundedChanged;
	}

	public override void _Process(double delta)
	{
		if(isGrounded){
			this.GlobalPosition = groundedTo.GlobalPosition;
		}else{
			this.Position = Vector2.Zero;
		}
	}

	private void OnPlayerGroundedChanged(bool newState, Planet groundedTo){
		isGrounded = newState;
		this.Zoom = isGrounded ? new Vector2(groundedZoom, groundedZoom) : new Vector2(normalZoom, normalZoom);
		this.groundedTo = groundedTo;
	}
}
