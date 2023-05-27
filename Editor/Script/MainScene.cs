using Godot;
using System;
using System.Collections.Generic;
using System.Reflection;

public partial class MainScene : Node2D
{

    [Export]
    public VisNode TestNode { get; set; }

    [Export]
    public VariableManager? VariableManager { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        TestNode.GenerateFunction(new FunctionInfo(false, new()
        {
            {"a", VisLang.ValueType.Number},
            {"b", VisLang.ValueType.Bool},
            {"banana", VisLang.ValueType.String}
        },
        VisLang.ValueType.Number, "VisLang.VariableSetNode"));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    private void Execute()
    {
        VisLang.VisSystem system = new VisLang.VisSystem();
        if (VariableManager != null)
        {
            foreach (VariableControl va in VariableManager.Variables)
            {
                if (string.IsNullOrWhiteSpace(va.VariableName))
                {
                    throw new Exception("Bad variable name!");
                }
                system.VisSystemMemory.CreateVariable(va.VariableName, va.VariableType);
            }
        }
        VisLang.VisNode? node = TestNode.CreateNode(system);
        if (node == null)
        {
            GD.PrintErr("Created node is not correct");
            return;
        }
		(node as VisLang.VariableSetNode).Name = VariableManager.Variables[0].VariableName; 
        system.Entrance = (node as VisLang.VariableSetNode);
        system.Execute();
		GD.Print(system.VisSystemMemory[VariableManager.Variables[0].VariableName].Data);
    }
}
