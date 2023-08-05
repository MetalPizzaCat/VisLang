using Godot;
using System;
using System.Linq;

/// <summary>
/// Control element that handles displaying input name and, if possible, control elements for manual value inputs
/// </summary>
public partial class EditorGraphManualInputControl : HBoxContainer
{

    public Label NameDisplayLabel { get; private set; }

    public int Slot { get; private set; }

    private bool _displayManualInput = true;

    private string _inputName = string.Empty;
    private VisLang.ValueType _valueType = VisLang.ValueType.Bool;

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
    public VisLang.ValueType InputType => _valueType;

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
            switch (_valueType)
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
        set
        {
            if (value == null)
            {
                return;
            }
            switch (InputType)
            {
                case VisLang.ValueType.Bool:
                    if (_inputControl is CheckBox checkBox)
                    {
                        checkBox.ButtonPressed = ((bool?)value) ?? false;
                    }
                    break;
                case VisLang.ValueType.Float:
                    if (_inputControl is SpinBox spinBox)
                    {
                        spinBox.Value = ((double?)value) ?? 0.0;
                    }
                    break;
                case VisLang.ValueType.Integer:
                    if (_inputControl is SpinBox intBox)
                    {
                        intBox.Value = ((long?)value) ?? 0;
                    }
                    break;
                case VisLang.ValueType.String:
                case VisLang.ValueType.Char:
                    if (_inputControl is LineEdit strEdit)
                    {
                        strEdit.Text = ((string?)value) ?? string.Empty;
                    }
                    break;
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
            _inputControl.Visible = HasManualInput;
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
                    Value = ((long?)defaultData) ?? 0
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
            _inputControl.Visible = HasManualInput;
        }
    }

    /// <summary>
    /// Creates a new instance of the control and if possible adds input controls to itself
    /// </summary>
    /// <param name="info">Input signature to use for generation</param>
    public EditorGraphManualInputControl(FunctionInputInfo info, int slot)
    {
        Slot = slot;
        _valueType = info.InputType;
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
        if (type != null && type.Value == InputType)
        {
            // no point in changing the type when it's already changed
            InputData = defaultData;
            return;
        }
        // remove old control, because there is no point in keeping it
        _inputControl?.QueueFree();
        RemoveChild(_inputControl);
        // add control back because yes
        CreateInputControl(type, defaultData);
        // also update the type
        _valueType = type ?? VisLang.ValueType.Bool;
    }
}