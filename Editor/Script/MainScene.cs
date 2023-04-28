using Godot;
using System;
using System.Collections.Generic;


public partial class MainScene : Node2D
{

	[Export]
	public VisNode TestNode { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TestNode.GenerateFunction(new FunctionInfo(false, new()
		{
			{"a", VisLang.ValueType.Number},
			{"b", VisLang.ValueType.Bool},
			{"banana", VisLang.ValueType.String}
		},
		VisLang.ValueType.Number));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
