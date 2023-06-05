using Godot;
using System;
using System.Collections.Generic;
using System.Linq;


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
        variable.Info.NameChanged += CheckAndNotifyVariableNames;
        VariableControlButtons.Add(variable);
        Container?.AddChild(variable);
        CheckAndNotifyVariableNames(variable.Info, variable.Info.Name, variable.Info.Name);
    }

    private void CheckAndNotifyVariableNames(VariableInfo? sender, string oldName, string newName)
    {
        foreach (VariableControl variable in VariableControlButtons)
        {
            // because event is called before the name change actually applies we have to do this little on the go check 
            // to use the correct name, otherwise it will always be slightly lagging behind
            string name = (variable.Info == sender) ? newName : variable.Info.Name;
            if (VariableControlButtons.Any(p => (sender == p.Info ? newName : p.Info.Name) == name && p != variable))
            {
                variable.DisplayDuplicateNameError();
            }
            else
            {
                variable.HideDuplicateNameError();
            }
        }

    }
}
