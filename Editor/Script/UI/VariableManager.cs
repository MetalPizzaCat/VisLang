namespace VisLang.Editor;

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// Information necessary to create a variable
/// </summary>
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

/// <summary>
/// Variable manager is a user control from which user can create, remove and edit variables present in the function
/// </summary>
public partial class VariableManager : Control
{
    public delegate void SetterRequestedEventHandler(VisLang.Editor.VariableInfo info);
    public delegate void GetterRequestedEventHandler(VisLang.Editor.VariableInfo info);
    public delegate void VariableDeletedEventHandler(VisLang.Editor.VariableInfo info);
    public delegate void VariableNameChangedEventHandler(VisLang.Editor.VariableInfo info, string name);
    public delegate void VariableTypeChangedEventHandler(VisLang.Editor.VariableInfo info, VisLang.ValueType type);

    /// <summary>
    /// Invoked when user presses button for creating a setter node
    /// </summary>
    public event SetterRequestedEventHandler? SetterRequested;
    /// <summary>
    /// Invoked when user presses button for creating a getter node
    /// </summary>
    public event GetterRequestedEventHandler? GetterRequested;

    public event VariableNameChangedEventHandler? VariableNameChanged;
    public event VariableTypeChangedEventHandler? VariableTypeChanged;

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
        // we don't really need to do anything when this happens so we let other objects handle these events 
        variable.SetterRequested += (VisLang.Editor.VariableInfo info) => { SetterRequested?.Invoke(info); };
        variable.GetterRequested += (VisLang.Editor.VariableInfo info) => { GetterRequested?.Invoke(info); };
        variable.TypeChanged += (VisLang.Editor.VariableInfo info, VisLang.ValueType type) => { VariableTypeChanged?.Invoke(info, type); };
        // but this one requires additional logic
        variable.NameChanged += CheckAndNotifyVariableNames;
        
        VariableControlButtons.Add(variable);
        Container?.AddChild(variable);
        CheckAndNotifyVariableNames(variable.Info, variable.Info.Name);
    }

    /// <summary>
    /// Checks variable names for duplication and updates variables controls to display error if invalid name is present
    /// </summary>
    /// <param name="sender">Info of the variable that was changed</param>
    /// <param name="newName">Name that variable was changed to</param>
    private void CheckAndNotifyVariableNames(VisLang.Editor.VariableInfo sender, string newName)
    {
        VariableNameChanged?.Invoke(sender, newName);
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
