using Godot;
using System;
using System.Collections.Generic;


public partial class VisNode : Node2D
{
	public FunctionInfo? FunctionInfo { get; set; } = null;

	[Export]
	public PackedScene? NodeInputPrefab { get; set; } = null;

	[Export]
	public Node2D NodeInputAnchor { get; set; }

	public List<NodeInput> Inputs { get; set; } = new List<NodeInput>();

	public override void _Ready()
	{
		if (FunctionInfo != null)
		{
			GenerateFunction(FunctionInfo);
		}
	}

	public void GenerateFunction(FunctionInfo info)
	{
		FunctionInfo = info;
		if (NodeInputPrefab == null)
		{
			throw new NullReferenceException("Unable to create node from description because input node was not provided");
		}
		float currentInputOffset = 0;
		foreach ((string name, VisLang.ValueType arg) in info.Inputs)
		{
			NodeInput input = NodeInputPrefab.InstantiateOrNull<NodeInput>();
			input.InputName = name;
			input.InputType = arg;
			Inputs.Add(input);
			NodeInputAnchor.AddChild(input);
			input.Position = new Vector2(0, currentInputOffset);
			currentInputOffset += 64f;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
