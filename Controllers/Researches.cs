using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Researches : Node
{
	public List<ResearchesEnum> researches = new List<ResearchesEnum>();

	public bool HaveResearch(ResearchesEnum research){
		return researches.FirstOrDefault(p => p == research) != null;
	}
}
