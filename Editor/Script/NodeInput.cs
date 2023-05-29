using Godot;
using System;

/// <summary>
/// Object that allows user to either input values by hand or connect to other inputs
/// </summary>
public partial class NodeInput : Node2D
{

    [Export]
    public Label InputNameLabel { get; set; }

    [ExportGroup("Input fields")]
    [Export]
    public LineEdit StringInput { get; set; }
    [Export]
    public SpinBox NumberInput { get; set; }
    [Export]
    public CheckBox BoolInput { get; set; }

    public VisNode? OwningNode { get; set; } = null;

    public string InputName
    {
        get => InputNameLabel.Text;
        set => InputNameLabel.Text = value;
    }

    private VisLang.ValueType _inputType = VisLang.ValueType.Bool;

    public VisLang.ValueType InputType
    {
        get => _inputType;
        set
        {
            StringInput.Visible = value == VisLang.ValueType.String;
            NumberInput.Visible = value == VisLang.ValueType.Number;
            BoolInput.Visible = value == VisLang.ValueType.Bool;
            _inputType = value;
        }
    }

    /// <summary>
    /// Get current value stored in the input. Resulting value will depend on InputType
    /// </summary>
    /// <value></value>
    public object Data
    {
        get
        {
            switch (_inputType)
            {
                case VisLang.ValueType.Bool:
                    return BoolInput.ButtonPressed;
                case VisLang.ValueType.Number:
                    // using float because i said so >:(
                    return (float)NumberInput.Value;
                case VisLang.ValueType.String:
                    return StringInput.Text;
            }
            return 0;
        }
    }
}
