using Godot;
using System;
using System.Collections.Generic;

public partial class VariableManager : Control
{
    public delegate void SetterRequestedEventHandler(VariableInfo info);
    public delegate void GetterRequestedEventHandler(VariableInfo info);
    public delegate void VariableDeletedEventHandler(VariableInfo info);

    public event SetterRequestedEventHandler? SetterRequested;
    public event GetterRequestedEventHandler? GetterRequested;

    [Export]
    public PackedScene? VariableControlPlaceholder { get; set; }
    [Export]
    public VBoxContainer? Container { get; set; }
    public List<VariableControl> VariableControlButtons { get; set; } = new();
    private void AddVariable()
    {
        VariableControl? variable = VariableControlPlaceholder?.InstantiateOrNull<VariableControl>();
        if (variable == null)
        {
            throw new Exception("Unable to create variable management control");
        }
        variable.SetterRequested += (VariableInfo info) => { SetterRequested?.Invoke(info); };
        variable.GetterRequested += (VariableInfo info) => { GetterRequested?.Invoke(info); };
        VariableControlButtons.Add(variable);
        Container?.AddChild(variable);
    }
}
