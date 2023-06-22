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
    public PopupMenu? ContextMenu { get; set; }

    [Export]
    public Control? ErrorDisplayControl { get; set; }

    private VariableInfo _info = new VariableInfo("Default", VisLang.ValueType.Bool);

    public VariableInfo Info => _info;

    public string VariableName => NameEdit.Text;

    public VisLang.ValueType VariableType => TypeOptionButton.Selected > -1 ? (VisLang.ValueType)TypeOptionButton.Selected : VisLang.ValueType.Bool;

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
        }
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


    private void ShowContextMenu()
    {
        if (ContextMenu != null)
        {
            ContextMenu.Position = new Vector2I((int)GetGlobalMousePosition().X, (int)GetGlobalMousePosition().Y);
            ContextMenu.Popup();
        }

    }

    private void ContextMenuItemSelected(int id)
    {
        // unfortunately the way context menu works is by passing the id of the selected item
        // which is not helpful when items are known before hand
        // 1 and 3 are  missing because those are the id of the group separators
        switch (id)
        {
            case 1: // get
                CreateGetter();
                return;
            case 2: // set
                CreateSetter();
                return;
            case 4: // get element at
                return;
            case 5: // set element at
                return;
        }
    }
}
