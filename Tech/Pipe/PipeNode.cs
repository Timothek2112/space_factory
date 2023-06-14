using Godot;
using System;
using System.Collections.Generic;

public partial class PipeNode : Area2D
{
	[Export] public bool isConnected;
	private PipeNode _connectedTo;
	[Export] public PipeNode connectedTo
	{
		get { return _connectedTo; }
		set { _connectedTo = value; isConnected = _connectedTo != null; }
	}
	[Export] public Items itemsType;
	[Export] public bool isAnyResource;
	public int itemsInPipe;
	[Export] public bool isInput;
	Line2D line;
	[Export] PackedScene linePrefab;
	PlayerController playerController;
	[Export] bool dragging;
	[Export] public float maxPipeLength = 10f;
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
		playerController.pipeInFocus = this;
	}

	private void _on_mouse_exited(){
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
		if (line != null)
			return;

		line = (Line2D)linePrefab.Instantiate();
		AddChild(line);
		
	}

	public void EndDragging(){
		dragging = false;
	}

	public void StartDragging(){
		if (!playerController.CanStartDragPipe())
			return;
		dragging = true;
		if(isConnected){
			connectedTo.Disconnect();
			Disconnect();
		}
	}

	public void Disconnect(){
		connectedTo = null;
		isConnected = false;
		if(line != null)
		{
			line.QueueFree();
			line = null;
		}
	}

	public void Connect(PipeNode to){
		connectedTo = to;
		to.connectedTo = this;
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
