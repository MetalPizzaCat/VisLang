using Godot;
using System;
using System.Collections.Generic;

public partial class VariableManager : Control
{
    [Export]
    public PackedScene VariableControlPlaceholder { get; set; }
    [Export]
    public VBoxContainer Container { get; set; }

    public List<VariableControl> Variables { get; set; } = new();

    private void AddVariable()
    {
		VariableControl variable = VariableControlPlaceholder.Instantiate<VariableControl>();
		Variables.Add(variable);
		Container.AddChild(variable);
    }
}
