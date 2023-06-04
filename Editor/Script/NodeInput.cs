using Godot;
using System;

/// <summary>
/// Object that allows user to either input values by hand or connect to other inputs
/// </summary>
public partial class NodeInput : Node2D
{

    public delegate void SelectedEventHandler(NodeInput input);
    public event SelectedEventHandler? Selected;
    [Export]
    public Label InputNameLabel { get; set; }

    [Export]
    public bool IsInput { get; set; } = true;
    [Export]
    public bool AllowsAny { get; set; } = false;

    [ExportGroup("Input fields")]
    [Export]
    public LineEdit StringInput { get; set; }
    [Export]
    public SpinBox NumberInput { get; set; }
    [Export]
    public CheckBox BoolInput { get; set; }

    public VisNode? OwningNode { get; set; } = null;

    /// <summary>
    /// Other side of the connection node
    /// </summary>
    public NodeInput? Connection { get; set; } = null;

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
            StringInput.Visible = value == VisLang.ValueType.String && IsInput;
            NumberInput.Visible = value == VisLang.ValueType.Number && IsInput;
            BoolInput.Visible = value == VisLang.ValueType.Bool && IsInput;
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

    private void Pressed()
    {
        Selected?.Invoke(this);
    }

    public bool IsValidTypeConnection(NodeInput other)
    {
        return (other.InputType == InputType) || AllowsAny;
    }

    /// <summary>
    /// Can this node connect to other node
    /// </summary>
    public bool CanConnect(NodeInput other)
    {
        // output can have as many connections as it wants, since it doesn't store any information about them
        bool connected = IsInput ? (Connection != null) : false;
        return !(connected || other.OwningNode == OwningNode || other.IsInput == IsInput || !IsValidTypeConnection(other));
    }
}
