using Godot;
using System;

public partial class PlayerController : Node
{
    [Export] public PipeNode pipe;
    [Export] public PipeNode pipeInFocus;
	[Export] public PlayerModeEnum mode = PlayerModeEnum.normal;
    [Export] public PlayerModeEnum prevMode = PlayerModeEnum.normal;
	public IBuildable buildingToBuild;
	public bool isDraggingPipe = false;
    [Export] public Planet groundedTo;

	public override void _Ready(){
		GetNode<CosmosController>("/root/CosmosController").PlayerGroundedChanged += OnPlayerGroundedChanged;
	}

	public void EnableBuildingMode(IBuildable building){
		if(mode == PlayerModeEnum.grounded){
			mode = PlayerModeEnum.building;
			buildingToBuild = building;
			((BaseBuilding)buildingToBuild).buildedOn = groundedTo;
			building.ShowBlueprint();
		}
	}

	public void DisableBuildingMode(){
		if(mode == PlayerModeEnum.building){
			mode = PlayerModeEnum.grounded;
			buildingToBuild = null;
		}
	}

	public void Build(){
		if(mode != PlayerModeEnum.building)
			return;

		buildingToBuild.Build();
		DisableBuildingMode();
    }

	public override void _Input(InputEvent @event){
		
		if(Input.IsActionJustPressed("Click")){
			if(pipeInFocus != null){
				if(pipeInFocus.isInput)
					return;
				pipeInFocus.StartDragging();
			}

            if (mode == PlayerModeEnum.building)
            {
                Build();
            }
        }
		if(Input.IsActionJustReleased("Click")){
			EndDragIfNoFocus();
			ConnectIfPossible();
        }
	}

	private void EndDragIfNoFocus(){
		if(pipeInFocus == null){
			if(isDraggingPipe)
				pipe.EndDragging();
			return;
		}
	}

	private void ConnectIfPossible(){
		if(pipeInFocus == null)
			return;
		if(!isDraggingPipe)
			return;
		if(pipeInFocus.isConnected){
			pipe.EndDragging();
			return;
		}

		pipe.Connect(to: pipeInFocus);
	}

	public void SetMode(PlayerModeEnum newMode){
		if (mode == newMode)
			return;
		prevMode = mode;
		mode = newMode;
	}

	public void StartDraggingPipe(PipeNode pipe){
		this.pipe = pipe;
		isDraggingPipe = true;
	}

	public void EndDraggingPipe(){
		this.pipe = null;
		isDraggingPipe = false;
	}

	public void SetGrounded(Planet planet){
		if(mode == PlayerModeEnum.normal){
			groundedTo = planet;
			SetMode(PlayerModeEnum.grounded);
			GetTree().GetFirstNodeInGroup("MainUI").GetNode<Control>("BuildingOptions").Visible = true;
		}
	}

	public void SetUngrounded(){
		groundedTo = null;
		ModeBack();
		GetTree().GetFirstNodeInGroup("MainUI").GetNode<Control>("BuildingOptions").Visible = false;
	}

	public void ModeBack(){
		mode = prevMode;
	}

	private void OnPlayerGroundedChanged(bool state, Planet groundedTo){
		if(state){
			SetGrounded(groundedTo);			
		}else{
			SetUngrounded();
		}
	}
}
