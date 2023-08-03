using Godot;
using System;
using System.Linq;

/// <summary>
/// Control element that handles displaying input name and, if possible, control elements for manual value inputs
/// </summary>
public partial class EditorGraphInputControl : HBoxContainer
{
    /// <summary>
    /// The input info that was used to construct the element
    /// </summary>
    public FunctionInputInfo Info { get; private set; }

    public Label NameDisplayLabel { get; private set; }

    public int Slot { get; private set; }

    private bool _displayManualInput = true;

    private string _inputName = string.Empty;

    /// <summary>
    /// If true control elements that allow user to type constants will be displayed on the element
    /// </summary>
    public bool HasManualInput
    {
        get => _displayManualInput;
        set
        {
            if (_inputControl != null)
            {
                _inputControl.Visible = value;
            }
            _displayManualInput = value;
        }
    }

    private Control? _inputControl = null;

    /// <summary>
    /// Type of the value that this input represents
    /// </summary>
    public VisLang.ValueType InputType => Info.InputType;

    /// <summary>
    ///  Data that was written by user into manual input field(default if there was an error getting data) or null if 
    /// given data type does not support manual input
    /// </summary>
    public object? InputData
    {
        get
        {
            if (_inputControl == null)
            {
                return null;
            }
            switch (Info.InputType)
            {
                case VisLang.ValueType.Bool:
                    return (_inputControl as CheckBox)?.ButtonPressed ?? false;
                case VisLang.ValueType.Float:
                    return (double)((_inputControl as SpinBox)?.Value ?? 0.0);
                case VisLang.ValueType.Integer:
                    return (int)((_inputControl as SpinBox)?.Value ?? 0);
                case VisLang.ValueType.String:
                    return (_inputControl as LineEdit)?.Text ?? string.Empty;
                case VisLang.ValueType.Char:
                    return (_inputControl as LineEdit)?.Text.FirstOrDefault() ?? ' ';
                default:
                    return null;
            }
        }
    }

    private void CreateInputControl(VisLang.ValueType? type, object? defaultData)
    {
        if (type == null)
        {
            // while any can be any, for the sake of simplicity we will make it a string
            _inputControl = new LineEdit();
            AddChild(_inputControl);
            return;
        }
        switch (type)
        {
            case VisLang.ValueType.Bool:
                _inputControl = new CheckBox() { ButtonPressed = ((bool?)defaultData) ?? false };
                break;
            case VisLang.ValueType.Float:
                _inputControl = new SpinBox()
                {
                    Step = 0f,
                    AllowGreater = true,
                    AllowLesser = true,
                    CustomArrowStep = 0.1f,
                    Value = ((double?)defaultData) ?? 0f
                };
                break;
            case VisLang.ValueType.Integer:
                _inputControl = new SpinBox()
                {
                    AllowGreater = true,
                    AllowLesser = true,
                    Value = ((int?)defaultData) ?? 0
                };
                break;
            case VisLang.ValueType.String:
                _inputControl = new LineEdit() { Text = ((string?)defaultData) ?? string.Empty };
                break;
            case VisLang.ValueType.Char:
                _inputControl = new LineEdit() { MaxLength = 1, Text = ((string?)defaultData) ?? "a" };
                break;
            default:
                NameDisplayLabel.Text = _inputName;
                break;
        }
        if (_inputControl != null)
        {
            AddChild(_inputControl);
        }
    }

    /// <summary>
    /// Creates a new instance of the control and if possible adds input controls to itself
    /// </summary>
    /// <param name="info">Input signature to use for generation</param>
    public EditorGraphInputControl(FunctionInputInfo info, int slot)
    {
        Slot = slot;
        Info = info;
        _inputName = info.InputName;
        NameDisplayLabel = new Label() { Text = $"{info.InputName}: " };
        AddChild(NameDisplayLabel);
        CreateInputControl(info.TypeMatchingPermissions == FunctionInputInfo.TypePermissions.AllowAny ? null : info.InputType, null);
    }


    /// <summary>
    /// Updates control to a new type and assigns default value to the manual input control
    /// </summary>
    /// <param name="type">New type of the input, if null "Any" is assumed</param>
    /// <param name="defaultData">The new data to put into manual input controls, ignored if data type can not be inputted manually </param>
    public void ChangeInputDataType(VisLang.ValueType? type, object? defaultData)
    {
        // remove old control, because there is no point in keeping it
        _inputControl?.QueueFree();
        RemoveChild(_inputControl);
        // add control back because yes
        CreateInputControl(type, defaultData);
    }
}