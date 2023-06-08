using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class Planet : Node2D
{
	[Export]
	public float weight = 5f;
	public List<object> spaceshipsAttached = new List<object>();
	[Export]
	public bool isHaveWater;
	[Export]
	public bool isHaveRock;
	[Export]
	public bool isHaveWood;
	public float radius = 1f;


	public override void _Ready()
	{
		radius = GetNode<Node2D>("BuildingPoint").GlobalPosition.DistanceTo(this.GlobalPosition);
	}

	public override void _Process(double delta)
	{
		foreach(var ship in spaceshipsAttached){
			var spaceship = (Spaceship)ship;
			if(spaceship.isGrounded)
				continue;
			var directionToPlanet = (this.GlobalPosition - spaceship.GlobalPosition).Normalized();
			var distanceFromShip = spaceship.GlobalPosition.DistanceTo(this.GlobalPosition);
			spaceship.velocity += directionToPlanet * (weight * spaceship.weight) / Mathf.Pow(distanceFromShip, 2);
		}
	}

	private void _on_area_2d_body_entered(Node2D body)
	{
		if(!body.IsInGroup("SpaceshipBody"))
			return;
		spaceshipsAttached.Add(body);
	}


	private void _on_area_2d_body_exited(Node2D body)
	{
		if(!body.IsInGroup("SpaceshipBody"))
			return;
		
		spaceshipsAttached.Remove(body);
	}

	private void _on_grounded_area_body_exited(Node2D body)
	{
		if(!body.IsInGroup("SpaceshipBody"))
			return;

		((Spaceship)body).SetGrounded(false, this);
	}

	private void _on_grounded_area_body_entered(Node2D body)
	{
		if(!body.IsInGroup("SpaceshipBody"))
			return;

		((Spaceship)body).SetGrounded(true, this);
	}
}
