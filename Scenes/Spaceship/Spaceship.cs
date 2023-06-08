using Godot;
using System;

public partial class Spaceship : CharacterBody2D
{
	PlayerController playerController;
	[Export]
	public Vector2 velocity = Vector2.Zero;
	[Export]
	public float stoppingFactor = 0.9f;
	[Export]
	public float acceleration = 0.3f;
	[Export]
	public float accelerationFactor = 0.5f;
	[Export]
	public float rotateSpeed = 0.1f;
	[Export]
	public float weight = 1;
	[Export]
	public bool isPlayer = false;
	[Export]
	private bool _isGrounded = false;
	[Export]
	public float boostForce = 50;

	public bool isGrounded 
	{
		get
		{
			return _isGrounded;
		}
	}
	private Planet groundedTo;

	public override void _Ready()
	{
		playerController = GetNode<PlayerController>("/root/PlayerController");
	}

	public override void _Process(double delta)
	{
		velocity = velocity * stoppingFactor;
		Move();
		if(_isGrounded)
			SetGroundedOrientation();
		this.MoveAndCollide(velocity / weight);
	}

	private void GroundedInput(){
		if(!isGrounded || !isPlayer)
			return;
		
		if(Input.IsKeyPressed(Key.Shift)){
			velocity += (this.GlobalPosition - groundedTo.GlobalPosition).Normalized() * boostForce;
		}
	}

	private void Move()
	{
		if(!isPlayer || _isGrounded){
			GroundedInput();
			return;
		}

		if(Input.IsKeyPressed(Key.W))
		{
			velocity += -acceleration * accelerationFactor * this.GlobalTransform.Y;
		}
		if(Input.IsKeyPressed(Key.S))
		{
			velocity += acceleration * accelerationFactor * this.GlobalTransform.Y;
		}
		if(Input.IsKeyPressed(Key.A))
		{
			velocity += acceleration * accelerationFactor * -this.GlobalTransform.X;
		}
		if(Input.IsKeyPressed(Key.D))
		{
			velocity += acceleration * accelerationFactor * this.GlobalTransform.X;
		}
		if(Input.IsKeyPressed(Key.Q) && !isGrounded)
		{
			this.Rotate(-rotateSpeed);
		}
		if(Input.IsKeyPressed(Key.E) && !isGrounded)
		{
			this.Rotate(rotateSpeed);
		}
	}

	private void SetGroundedOrientation()
	{
		var directionFromGround = this.GlobalPosition - groundedTo.GlobalPosition;
		this.Rotation = directionFromGround.Angle() + Mathf.Pi/2;
	}

	public void SetGrounded(bool state, Planet groundedTo)
	{
		_isGrounded = state;
		this.groundedTo = state ? groundedTo : null;
		velocity = Vector2.Zero;
		if(isPlayer)
			GetNode<CosmosController>("/root/CosmosController").Call_PlayerGroundedChanged(state, this.groundedTo);
	}
}
