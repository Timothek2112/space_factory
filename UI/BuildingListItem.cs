using System.Collections.Generic;
using System.Linq;
using Godot;
using System;
using Newtonsoft.Json;


public partial class BuildingListItem : Button
{
	[Export] public int id;
	[Export] public PackedScene building;
	[Export] BuildingTypesEnum buildingType;
	public List<ResearchesEnum> researches = new List<ResearchesEnum>();

	public override void _Ready(){
		var json = (Json)GD.Load("res://Tech/Config/buildingsShop.tres");
		var buildings = JsonConvert.DeserializeObject<List<BuildingListItem>>(json.Data.ToString());
		var self = buildings.FirstOrDefault(p => p.id == id);
		if(self == null)
			return;
		researches = self.researches;
		buildingType = self.buildingType;
	}

	private void _on_button_down(){
		var instance = building.Instantiate();
		GetNode("/root").AddChild(instance);
		GetNode<PlayerController>("/root/PlayerController").EnableBuildingMode((IBuildable)instance);
	}
}
