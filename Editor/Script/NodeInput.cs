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
    public CodeColorTheme? Theme { get; set; }
    [Export]
    public Label InputNameLabel { get; set; }

    [Export]
    public bool IsInput { get; set; } = true;
    [Export]
    public Sprite2D DefaultIcon { get; set; }
    [Export]
    public Sprite2D ArrayIcon { get; set; }


    [Export]
    public FunctionInputInfo.TypePermissions TypeMatchingPermissions { get; set; } = FunctionInputInfo.TypePermissions.SameTypeOnly;

    [ExportGroup("Input fields")]
    [Export]
    public LineEdit StringInput { get; set; }
    [Export]
    public SpinBox NumberInput { get; set; }
    [Export]
    public CheckBox BoolInput { get; set; }
    [Export]
    public SpinBox IntInput { get; set; }

    public EditorVisNode? OwningNode { get; set; } = null;

    /// <summary>
    /// Other side of the connection node
    /// </summary>
    public NodeInput? Connection { get; set; } = null;

    public bool IsInputConnected => Connection != null;

    public string InputName
    {
        get => InputNameLabel.Text;
        set => InputNameLabel.Text = value;
    }

    private VisLang.ValueType _inputType = VisLang.ValueType.Bool;
    private bool _isArray = false;

    public VisLang.ValueType InputType
    {
        get => _inputType;
        set
        {
            // inputs are not visible if it's not an input, but also invisible if it's an array since we don't provide a way to edit arrays via editor
            // because that would be too annoying to add 
            StringInput.Visible = value == VisLang.ValueType.String && IsInput && !IsArray;
            NumberInput.Visible = value == VisLang.ValueType.Float && IsInput && !IsArray;
            BoolInput.Visible = value == VisLang.ValueType.Bool && IsInput && !IsArray;
            IntInput.Visible = value == VisLang.ValueType.Integer && IsInput && !IsArray;
            if (Theme != null)
            {
                DefaultIcon.SelfModulate = TypeMatchingPermissions == FunctionInputInfo.TypePermissions.AllowAny ? Theme.AnyColor : Theme.GetColorForType(value);
                ArrayIcon.SelfModulate = TypeMatchingPermissions == FunctionInputInfo.TypePermissions.AllowAny ? Theme.AnyColor : Theme.GetColorForType(value);
            }
            _inputType = value;
        }
    }

    public bool IsArray => InputType == VisLang.ValueType.Array;


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
                case VisLang.ValueType.Float:
                    // using float because i said so >:(
                    return (float)NumberInput.Value;
                case VisLang.ValueType.String:
                    return StringInput.Text;
                case VisLang.ValueType.Integer:
                    // godot uses double for spin box but int can't be double can it?
                    return (int)IntInput.Value;
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
        // check if types are known at design time and if they are ensure that they match
        // if type can not be known at design time we allow the connection and make runtime deal with it
        // if input does not care what type is passed into we also allow the connection and make it runtime's job to deal with errors  
        return (other.InputType == InputType && other.IsArray == IsArray) ||
         (TypeMatchingPermissions == FunctionInputInfo.TypePermissions.AllowAny || other.TypeMatchingPermissions == FunctionInputInfo.TypePermissions.AllowAny);
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
