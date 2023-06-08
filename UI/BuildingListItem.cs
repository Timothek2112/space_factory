using System.Collections.Generic;
using System.Linq;
using Godot;
using System;
using Newtonsoft.Json;


public partial class BuildingListItem : Button
{
	[Export] public int id;
	[Export] public int building_id;
	public IBuildable building;
	[Export] BuildingTypesEnum buildingType;
	public List<ResearchesEnum> researches = new List<ResearchesEnum>();

	public override void _Ready(){
		var json = (Json)GD.Load("res://Tech/Config/buildingsShop.tres");
		var buildings = JsonConvert.DeserializeObject<List<BuildingListItem>>(json.Data.ToString());
		var self = buildings.FirstOrDefault(p => p.id == id);
		if(self == null)
			return;
		researches = self.researches;
		building_id = self.building_id;
		buildingType = self.buildingType;
		if(buildingType == BuildingTypesEnum.mine)
		{
			json = (Json)GD.Load("res://Tech/Config/Mines.tres");
			var concreteBuildings = JsonConvert.DeserializeObject<List<Mine>>(json.Data.ToString());
			var thisBuilding = concreteBuildings.FirstOrDefault(p => p.id == building_id);
			if(thisBuilding == null)
				return;
			building = thisBuilding;
		}
		else{
			json = (Json)GD.Load("res://Tech/Config/Storages.tres");
			var concreteBuildings = JsonConvert.DeserializeObject<List<StorageBuilding>>(json.Data.ToString());
			var thisBuilding = concreteBuildings.FirstOrDefault(p => p.id == building_id);
			if(thisBuilding == null)
				return;
			building = thisBuilding;
		}
		
	}

	private void _on_button_down(){
		var newBuilding = new PackedScene();
		newBuilding.Pack((Node)building);
		var instance = newBuilding.Instantiate();
		AddChild(instance);
		((BaseBuilding)instance).Builded = instance.GetNode<Node2D>("Builded");
		((BaseBuilding)instance).Blueprint = instance.GetNode<Node2D>("Blueprint");
		GetNode<PlayerController>("/root/PlayerController").EnableBuildingMode((IBuildable)instance);

	}
}
