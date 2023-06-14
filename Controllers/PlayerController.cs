using Godot;
using System;

public partial class PlayerController : Node
{
	[Export] public bool isDraggingPipe = false;
    [Export] private PipeNode _pipe;
	public PipeNode pipe
    {
		get { return _pipe; }
		set { _pipe = value; isDraggingPipe = value != null; }
	}

	public bool isDraggingItem = false;
	private ItemUIDraggable _draggingItem;
	public ItemUIDraggable draggingItem
	{
		get { return _draggingItem; }
		set { _draggingItem = value; isDraggingItem = value != null; }
	}

	[Export ]public bool isUIinFocus;
	private StorageInventory _inventoryInFocus;
	public StorageInventory inventoryInFocus
	{
		get { return _inventoryInFocus; }
		set { _inventoryInFocus = value; isUIinFocus = value != null; }
	}

	[Export] public PipeNode pipeInFocus;
	[Export] public PlayerModeEnum mode = PlayerModeEnum.normal;
	[Export] public PlayerModeEnum prevMode = PlayerModeEnum.normal;
	public IBuildable buildingToBuild;
	[Export] public Planet groundedTo;

	public override void _Ready() {
		GetNode<CosmosController>("/root/CosmosController").PlayerGroundedChanged += OnPlayerGroundedChanged;
	}

	public void EnableBuildingMode(IBuildable building) {
		if (mode == PlayerModeEnum.grounded) {
			mode = PlayerModeEnum.building;
			buildingToBuild = building;
			((BaseBuilding)buildingToBuild).buildedOn = groundedTo;
			building.ShowBlueprint();
		}
	}

	public void DisableBuildingMode() {
		if (mode == PlayerModeEnum.building) {
			mode = PlayerModeEnum.grounded;
			buildingToBuild = null;
		}
	}

	public void TryBuild() {
		if (mode != PlayerModeEnum.building)
			return;
		if (buildingToBuild == null)
			return;
		buildingToBuild.Build();
		DisableBuildingMode();
	}

	public override void _Input(InputEvent @event) {

		if (Input.IsActionJustPressed("Click")) {
			if (pipeInFocus != null) {
				if (pipeInFocus.isInput)
					return;
				StartDraggingPipe(pipeInFocus);
			}

			TryBuild();
		}
		if (Input.IsActionJustReleased("Click")) {
			GD.Print(_pipe);
			GD.Print(isDraggingPipe);
			EndDragIfNoFocus();
			ConnectIfPossible();
		}
	}

	private void EndDragIfNoFocus() {
		if (!isDraggingPipe)
			return;
        if (pipeInFocus != null)
			return;

        EndDraggingPipe();
    }

    private void ConnectIfPossible() {
		if (pipeInFocus == null)
			return;
		if (!isDraggingPipe)
			return;
		if (pipeInFocus.isConnected) {
			EndDraggingPipe();
			return;
		}

		pipe.Connect(to: pipeInFocus);
	}

	public void SetMode(PlayerModeEnum newMode) {
		if (mode == newMode)
			return;
		prevMode = mode;
		mode = newMode;
	}

	public bool CanStartDragPipe()
	{
        if (isDraggingItem)
            return false;
		if (isUIinFocus)
			return false;
		return true;
    }

	public void StartDraggingPipe(PipeNode pipe) {

		pipe.StartDragging();
		this.pipe = pipe;
	}

	public void EndDraggingPipe() {
		pipe.EndDragging();
		this.pipe = null;
	}

	public void SetGrounded(Planet planet) {
		if (mode == PlayerModeEnum.normal) {
			groundedTo = planet;
			SetMode(PlayerModeEnum.grounded);
			GetTree().GetFirstNodeInGroup("MainUI").GetNode<Control>("BuildingOptions").Visible = true;
		}
	}

	public void SetUngrounded() {
		groundedTo = null;
		ModeBack();
		GetTree().GetFirstNodeInGroup("MainUI").GetNode<Control>("BuildingOptions").Visible = false;
	}

	public void ModeBack() {
		mode = prevMode;
	}

	private void OnPlayerGroundedChanged(bool state, Planet groundedTo) {
		if (state) {
			SetGrounded(groundedTo);
		} else {
			SetUngrounded();
		}
	}

	public bool CanStartDragItem()
	{
        if (isDraggingPipe)
            return false;
		if (isDraggingItem)
			return false;
		return true;
    }

    public void StartDraggingItem(ItemUIDraggable item)
	{
		draggingItem = item;
	}

	public void EndDraggingItem()
	{
		draggingItem = null;
	}
}
