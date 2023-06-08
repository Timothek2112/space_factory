using Godot;
using System;
using System.Collections.Generic;

public partial class PipeNode : Area2D
{
	[Export]
	public bool isConnected;
	[Export]
	public PipeNode connectedTo;
	[Export]
	public Items itemsType;
	[Export]
	public bool isAnyResource;
	public int itemsInPipe;
	[Export]
	bool mouseIn = false;
	[Export]
	public bool isInput;
	Line2D line;
	[Export]
	PackedScene linePrefab;
	PlayerController playerController;
	[Export]
	bool dragging;
	[Export]
	public float maxPipeLength = 10f;
	public IPutable origin;

	public override void _Ready()
	{
		playerController = GetNode<PlayerController>("/root/PlayerController");
		origin = this.GetParent<IPutable>();
	}

	public override void _Process(double delta)
	{
		LineDragging();
		LineCheck();
		ProcessItem();
	}

	private void _on_mouse_entered(){
		mouseIn = true;
		playerController.pipeInFocus = this;
	}

	private void _on_mouse_exited(){
		mouseIn = false;
		playerController.pipeInFocus = null;
	}

	

	private void LineDragging(){
		if(!dragging)
			return;

		CreateLineIfNull();
		
		var points = new Vector2[] { Vector2.Zero, (GetGlobalMousePosition() - this.GlobalPosition).Rotated(-this.GetParent<Mine>().Rotation)};
		var pointsDistance = points[0].DistanceTo(points[1]);
		if(maxPipeLength <= pointsDistance){
			var del = maxPipeLength / pointsDistance;
			points[1] *= del;
		}
		line.Points = points;
	}

	private void LineCheck(){
		if(!isConnected && !dragging && line != null){
			line.QueueFree();
			line = null;
		}

		if(isConnected && !isInput){
			CreateLineIfNull();
			var points = new Vector2[] {Vector2.Zero, connectedTo.GlobalPosition - this.GlobalPosition};
			line.Points = points;
		}
	}

	private void CreateLineIfNull(){
		if(line == null){
			line = (Line2D)linePrefab.Instantiate();
			AddChild(line);
		}
	}

	public void EndDragging(){
		dragging = false;
		playerController.EndDraggingPipe();
	}

	public void StartDragging(){
		dragging = true;
		playerController.StartDraggingPipe(this);
		if(isConnected){
			connectedTo.Disconnect();
			Disconnect();
		}
	}

	public void Disconnect(){
		if(!isConnected)
			return;
		connectedTo = null;
		isConnected = false;
		if(line != null)
		{
			line.QueueFree();
			line = null;
		}
	}

	public void Connect(PipeNode to){
		isConnected = true;
		connectedTo = to;
		to.connectedTo = this;
		to.isConnected = true;
	}

	public void ProcessItem(){
		if(!isConnected)
			return;

		if(isInput)
			InputProcessing();
		else
			OutputProcessing();
	}

	public void InputProcessing()
	{
		connectedTo.GiveItem(to: this);
		if(itemsInPipe <= 0)
			return;
		if(!origin.CanAcceptItem(itemsType, 1))
			return;
		origin.PutItem(from: this);
	}

	public void OutputProcessing()
	{
		if(!connectedTo.origin.CanAcceptItem(itemsType, 1))
			return;
		origin.GiveItem(to: this); // TODO: Добавить время на передачу
	}

	public void GiveItem(PipeNode to){
		if(to.itemsType != itemsType && !to.isAnyResource)
			return; // TODO: Пока нихуя не происходит но скорее всего надо как-то уведомлять игрока, что трубы не правильные
		if(itemsInPipe <= 0)
			return;
		to.itemsType = itemsType;
		to.itemsInPipe += 1;
		itemsInPipe -= 1;
	}
}
