using Godot;
using System;

public partial class CosmosController : Node
{
	public delegate void PlayerGroundedChangedArgs(bool newState, Planet groundedTo);
	public event PlayerGroundedChangedArgs PlayerGroundedChanged;

	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	public void Call_PlayerGroundedChanged(bool newState, Planet groundedTo){
		PlayerGroundedChanged?.Invoke(newState, groundedTo);
	}
}
