using Godot;
using System;
using System.Text.RegularExpressions;


public partial class VariableControl : HBoxContainer
{
    public delegate void SetterRequestedEventHandler(VisLang.Editor.VariableInfo info);
    public delegate void GetterRequestedEventHandler(VisLang.Editor.VariableInfo info);

    public delegate void NameChangedEventHandler(VisLang.Editor.VariableInfo info, string newName);
    public delegate void TypeChangedEventHandler(VisLang.Editor.VariableInfo info, VisLang.ValueTypeData type);

    public event SetterRequestedEventHandler? SetterRequested;
    public event GetterRequestedEventHandler? GetterRequested;
    public event NameChangedEventHandler? NameChanged;
    public event TypeChangedEventHandler? TypeChanged;


    [Export]
    public LineEdit NameEdit { get; set; }

    [Export]
    public OptionButton TypeOptionButton { get; set; }
    [Export]
    public OptionButton ArrayTypeOptionButton { get; set; }
    [Export]
    public PopupMenu? ContextMenu { get; set; }

    [Export]
    public Control? ErrorDisplayControl { get; set; }

    private VisLang.Editor.VariableInfo _info = new VisLang.Editor.VariableInfo("Default", VisLang.ValueType.Bool, null, false);

    public VisLang.Editor.VariableInfo Info => _info;

    public string VariableName => NameEdit.Text;

    public VisLang.ValueType VariableType => TypeOptionButton.Selected > -1 ? (VisLang.ValueType)TypeOptionButton.Selected : VisLang.ValueType.Bool;
    public VisLang.ValueType? ArrayDataType => ArrayTypeOptionButton.Selected > -1 && ArrayTypeOptionButton.Selected < Enum.GetNames(typeof(VisLang.ValueType)).Length ? (VisLang.ValueType)ArrayTypeOptionButton.Selected : null;

    private Regex _variableNameCheckRegex = new Regex(@"^(([A-z])+[0-9]*)$");

    private bool _isInvalidName = false;
    private bool _isNameDuplicate = false;

    public bool IsInvalidName
    {
        get => _isInvalidName;
        set
        {
            _isInvalidName = value;
            DisplayErrors();
        }
    }

    public bool IsNameDuplicate
    {
        get => _isNameDuplicate;
        set
        {
            _isNameDuplicate = value;
            DisplayErrors();
        }
    }

    public override void _Ready()
    {
        foreach (VisLang.ValueType val in (VisLang.ValueType[])Enum.GetValues(typeof(VisLang.ValueType)))
        {
            TypeOptionButton.AddItem(val.ToString());
            ArrayTypeOptionButton.AddItem(val.ToString());
        }
        ArrayTypeOptionButton.AddItem("Any");
    }

    private void ChangeName(string newName)
    {
        if (ErrorDisplayControl != null)
        {
            if (!_variableNameCheckRegex.IsMatch(newName))
            {
                IsInvalidName = true;
            }
            else
            {

                IsInvalidName = false;
            }
        }

        _info.Name = newName;
        NameChanged?.Invoke(_info, newName);
    }

    public void DisplayDuplicateNameError()
    {
        IsNameDuplicate = true;
    }

    public void HideDuplicateNameError()
    {
        IsNameDuplicate = false;
    }

    private void DisplayErrors()
    {
        if (ErrorDisplayControl == null)
        {
            return;
        }
        if (IsInvalidName)
        {
            ErrorDisplayControl.Visible = true;
            ErrorDisplayControl.TooltipText = "Invalid variable name. Name can not contain spaces, special characters and must not start with a number";
        }
        else if (IsNameDuplicate)
        {
            ErrorDisplayControl.Visible = true;
            ErrorDisplayControl.TooltipText = "Name is already in use";
        }
        else
        {
            ErrorDisplayControl.Visible = false;
        }
    }

    private void SelectType(int type)
    {
        _info.Type = (VisLang.ValueType)type;
        ArrayTypeOptionButton.Visible = _info.Type == VisLang.ValueType.Array;
        _info.IsArray = _info.Type == VisLang.ValueType.Array;
        if (_info.IsArray && _info.ArrayDataType == null)
        {
            _info.ArrayDataType = VisLang.ValueType.Bool;
        }
        TypeChanged?.Invoke(_info, new VisLang.ValueTypeData(_info.Type, _info.ArrayDataType));
    }

    private void CreateSetter()
    {
        SetterRequested?.Invoke(Info);
    }

    private void CreateGetter()
    {
        GetterRequested?.Invoke(Info);
    }


    private void ShowContextMenu()
    {
        if (ContextMenu != null)
        {
            ContextMenu.Position = new Vector2I((int)GetGlobalMousePosition().X, (int)GetGlobalMousePosition().Y);
            ContextMenu.Popup();
        }

    }

    private void SetArrayType(int type)
    {
        // type is either a type or if 'Any' is selected it is a null value
        // since any exists outside of the enum range we just check for that
        _info.ArrayDataType = (type < Enum.GetNames(typeof(VisLang.ValueType)).Length) ? ((VisLang.ValueType)type) : null;
        TypeChanged?.Invoke(_info, new VisLang.ValueTypeData(_info.Type, _info.ArrayDataType));
    }
}
