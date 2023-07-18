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

    private bool _displayManualInput = true;

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

    /// <summary>
    /// Creates a new instance of the control and if possible adds input controls to itself
    /// </summary>
    /// <param name="info">Input signature to use for generation</param>
    public EditorGraphInputControl(FunctionInputInfo info)
    {
        Info = info;
        NameDisplayLabel = new Label() { Text = $"{info.InputName}: " };
        AddChild(NameDisplayLabel);
        if (info.TypeMatchingPermissions == FunctionInputInfo.TypePermissions.AllowAny)
        {
            // while any can be any, for the sake of simplicity we will make it a string
            _inputControl = new LineEdit();
            AddChild(_inputControl);
            return;
        }
        switch (info.InputType)
        {
            case VisLang.ValueType.Bool:
                _inputControl = new CheckBox();
                break;
            case VisLang.ValueType.Float:
                _inputControl = new SpinBox() { Step = 0f, AllowGreater = true, AllowLesser = true, CustomArrowStep = 0.1f };
                break;
            case VisLang.ValueType.Integer:
                _inputControl = new SpinBox() { AllowGreater = true, AllowLesser = true };
                break;
            case VisLang.ValueType.String:
                _inputControl = new LineEdit();
                break;
            case VisLang.ValueType.Char:
                _inputControl = new LineEdit() { MaxLength = 1 };
                break;
            default:
                NameDisplayLabel.Text = info.InputName;
                break;
        }
        if (_inputControl != null)
        {
            AddChild(_inputControl);
        }
    }
}