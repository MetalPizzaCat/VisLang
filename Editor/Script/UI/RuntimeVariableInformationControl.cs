using Godot;
using System;
using System.Collections.Generic;


public partial class RuntimeVariableInformationControl : Control
{
    [Export]
    public VBoxContainer InformationContainer { get; set; }

    private List<Label> _variableInfoLabels = new();

    public void DisplayInformation(VisLang.VisSystemMemory memory)
    {
        foreach (Label label in _variableInfoLabels)
        {
            InformationContainer.RemoveChild(label);
            label.QueueFree();
        }
        _variableInfoLabels.Clear();
        foreach ((string name, uint address) in memory.Variables)
        {
            Label label = new Label() { Text = $"{name}: {memory.Memory[address].Data}" };
			_variableInfoLabels.Add(label);
			InformationContainer.AddChild(label);
        }
    }
}
