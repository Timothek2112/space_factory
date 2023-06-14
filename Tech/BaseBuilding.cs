using Godot;
using static Godot.GD;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

public partial class BaseBuilding : Node2D, IPutable, IBuildable
{
	[Export]
	public int id;
	public string name;
	public List<Slot> inputSlots = new List<Slot>();
	public List<Slot> outputSlots = new List<Slot>();
	[Export] public Planet buildedOn;
	[Export] public Node2D Builded;
	[Export] public Node2D Blueprint;
	public float angleOnPlanet;
	[Export] public PackedScene inventoryScene;
	[Export] public bool built { get; set; }
	PlayerController playerController;

	public override void _Ready()
	{
		base._Ready();
		playerController = GetNode<PlayerController>("/root/PlayerController");
	}

	public override void _Process(double delta) {
		base._Process(delta);
		SetPosition();
		if (built && buildedOn == null)
		{
			ShowBlueprint();
		}
	}

	public void SetPosition() {
		if (built)
			SetPositionOnPlanet();
		else
			SetPositionNotBuilt();
	}

	public void SetPositionNotBuilt() {
		var directionFromGround = GetGlobalMousePosition() - buildedOn.GlobalPosition;
		GlobalPosition = (GetGlobalMousePosition() - buildedOn.GlobalPosition).Normalized() * buildedOn.radius + buildedOn.GlobalPosition;
		this.Rotation = angleOnPlanet = directionFromGround.Angle() + Mathf.Pi / 2;
	}

	public void SetPositionOnPlanet() {
		GlobalPosition = (buildedOn.GlobalPosition + Vector2.Up.Rotated(angleOnPlanet) * buildedOn.radius);
		var directionFromGround = this.GlobalPosition - buildedOn.GlobalPosition;
		this.Rotation = directionFromGround.Angle() + Mathf.Pi / 2;
	}

	public virtual void GiveItem(PipeNode to) {
		if (!to.isConnected)
			return;
		var itemsForPipe = outputSlots.FirstOrDefault(p => p.type == to.itemsType);
		if (itemsForPipe == null)
			return;
		if (itemsForPipe.currentCount > 0) {
			to.itemsInPipe += 1;
			itemsForPipe.currentCount -= 1;
		}
	}

	public virtual void PutItem(PipeNode from) {
		if (from.itemsInPipe <= 0)
			return;

		var storageForItem = inputSlots.FirstOrDefault(p => p.type == from.itemsType && p.currentCount < p.capability);
		if (storageForItem == null)
			return;

		from.itemsInPipe -= 1;
		storageForItem.currentCount += 1;
	}

	public virtual bool CanAcceptItem(Items type, int count) {
		var storageForItem = inputSlots.FirstOrDefault(p => p.type == type && p.currentCount < p.capability);
		if (storageForItem == null)
			return false;
		return true;
	}

	public void Build()
	{
		if (buildedOn == null)
			return;
		built = true;
		Builded.Visible = true;
		Blueprint.Visible = false;
	}

	public void ShowBlueprint()
	{
		built = false;
		Builded.Visible = false;
		Blueprint.Visible = true;
	}

	public void _on_touch_area_input_event(Node viewport, InputEvent @event, int shape_idx)
	{
		if (@event.IsActionReleased("Click"))
		{
			ShowInventory();
        }
	}

	public virtual void ShowInventory()
	{
        var instance = (StorageInventory)inventoryScene.Instantiate();
        GetTree().GetFirstNodeInGroup("MainCanvas").AddChild(instance);
        instance.SetSlots(ref inputSlots, inputSlots.Count);
        instance.ShowStoreInventory();
    }
}
