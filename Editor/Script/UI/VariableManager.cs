namespace VisLang.Editor;

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


public class VariableInitInfo
{
    public VariableInitInfo(string name, ValueTypeData type)
    {
        Name = name;
        Type = type;
    }

    public string Name { get; set; }

    public ValueTypeData Type { get; set; }

}

public partial class VariableManager : Control
{
    public delegate void SetterRequestedEventHandler(VisLang.Editor.VariableInfo info);
    public delegate void GetterRequestedEventHandler(VisLang.Editor.VariableInfo info);
    public delegate void VariableDeletedEventHandler(VisLang.Editor.VariableInfo info);

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
        variable.SetterRequested += (VisLang.Editor.VariableInfo info) => { SetterRequested?.Invoke(info); };
        variable.GetterRequested += (VisLang.Editor.VariableInfo info) => { GetterRequested?.Invoke(info); };
        variable.NameChanged += CheckAndNotifyVariableNames;
        VariableControlButtons.Add(variable);
        Container?.AddChild(variable);
        CheckAndNotifyVariableNames(variable.Info, variable.Info.Name);
    }

    private void CheckAndNotifyVariableNames(VisLang.Editor.VariableInfo sender, string newName)
    {
        foreach (VariableControl variable in VariableControlButtons)
        {
            if (VariableControlButtons.Any(p => p.Info != sender && p.VariableName == newName))
            {
                variable.DisplayDuplicateNameError();
            }
            else
            {
                variable.HideDuplicateNameError();
            }
        }

    }

    public List<VariableInitInfo> GetVariableInits()
    {
        return VariableControlButtons.Select(btn => new VariableInitInfo(btn.VariableName, new ValueTypeData(btn.VariableType, btn.ArrayDataType))).ToList();
    }
}
