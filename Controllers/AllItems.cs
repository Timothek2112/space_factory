using static Godot.GD;
using Godot;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

public partial class AllItems : Node
{
	List<Item> items = new List<Item>();

	public override void _Ready(){
		ParseAndSave();
	}

	public void ParseAndSave(){
		var json = GD.Load("res://Items/AllItems.tres");
		items = JsonConvert.DeserializeObject<List<Item>>(((Json)json).Data.ToString());
	}
}
