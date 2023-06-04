using Godot;
using System;
using System.Text.RegularExpressions;


public partial class VariableControl : HBoxContainer
{
    public delegate void SetterRequestedEventHandler(VariableInfo info);
    public delegate void GetterRequestedEventHandler(VariableInfo info);

    public event SetterRequestedEventHandler? SetterRequested;
    public event GetterRequestedEventHandler? GetterRequested;


    [Export]
    public LineEdit NameEdit { get; set; }

    [Export]
    public OptionButton TypeOptionButton { get; set; }

    [Export]
    public Control? ErrorDisplayControl { get; set; }

    private VariableInfo _info = new VariableInfo("Default", VisLang.ValueType.Bool);

    public VariableInfo Info => _info;

    public string VariableName => NameEdit.Text;

    public VisLang.ValueType VariableType => TypeOptionButton.Selected > -1 ? (VisLang.ValueType)TypeOptionButton.Selected : VisLang.ValueType.Bool;

    private Regex _variableNameCheckRegex = new Regex(@"^(([A-z])+[0-9]*)$");

    public override void _Ready()
    {
        foreach (VisLang.ValueType val in (VisLang.ValueType[])Enum.GetValues(typeof(VisLang.ValueType)))
        {
            TypeOptionButton.AddItem(val.ToString());
        }
    }

    private void ChangeName(string newName)
    {
        if (ErrorDisplayControl != null)
        {
            if (!_variableNameCheckRegex.IsMatch(newName))
            {
                ErrorDisplayControl.Visible = true;
                ErrorDisplayControl.TooltipText = "Invalid variable name. Name can not contain spaces, special characters and must not start with a number";
            }
            else
            {
                ErrorDisplayControl.Visible = false;
            }
        }

        _info.Name = newName;
    }

    private void SelectType(int type)
    {
        _info.ValueType = (VisLang.ValueType)type;
    }

    private void CreateSetter()
    {
        SetterRequested?.Invoke(Info);
    }

    private void CreateGetter()
    {
        GetterRequested?.Invoke(Info);
    }
}
