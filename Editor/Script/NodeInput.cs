using Godot;
using System;

/// <summary>
/// Object that allows user to either input values by hand or connect to other inputs
/// </summary>
public partial class NodeInput : Node2D
{
    public delegate void DestroyedEventHandler(NodeInput sender);
    public delegate void ConnectionCreatedEventHandler(NodeInput sender, NodeInput other);
    public delegate void ConnectionDestroyedEventHandler(NodeInput sender);
    public event ConnectionCreatedEventHandler? ConnectionCreated;
    public event ConnectionDestroyedEventHandler? ConnectionDestroyed;
    /// <summary>
    /// Called when node is being destroyed
    /// </summary>
    public event DestroyedEventHandler? Destroyed;

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
    public Sprite2D ErrorIcon { get; set; }


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

    private NodeInput? _connection = null;

    private bool _valid = true;

    public bool Valid
    {
        get => _valid;
        set
        {
            _valid = value;
            ErrorIcon.Visible = !value;
        }
    }


    /// <summary>
    /// Other side of the connection node
    /// </summary>
    public NodeInput? Connection
    {
        get => _connection;
        set
        {
            NodeInput? old = _connection;
            _connection = value;
            // if was and now is not means connection destroyed 
            if (old != null && _connection == null)
            {
                ConnectionDestroyed?.Invoke(this);
            }
            // if was not and now is means connection created
            if (old == null && _connection != null)
            {
                ConnectionCreated?.Invoke(this, _connection);
            }
            UpdateInputsVisuals();
        }
    }

    public bool IsManualInputVisible => !IsInputConnected && IsInput && !IsArray && Valid;

    public bool IsInputConnected => Connection != null;

    public string InputName
    {
        get => InputNameLabel.Text;
        set => InputNameLabel.Text = value;
    }

    private VisLang.ValueType _inputType = VisLang.ValueType.Bool;
    private VisLang.ValueType? _arrayDataType = null;
    /// <summary>
    /// If true given input should change it's type based on parent node array type
    /// </summary>
    [Export]
    public bool IsArrayTypeDependent { get; set; } = false;

    public VisLang.ValueType InputType
    {
        get => _inputType;
        set
        {
            _inputType = value;
            UpdateInputsVisuals();
        }
    }

    /// <summary>
    /// Data type of the typed array. Only applies if InputType is Array
    /// </summary>
    /// <value></value>
    public VisLang.ValueType? ArrayDataType
    {
        get => _arrayDataType;
        set
        {
            _arrayDataType = value;
            UpdateInputsVisuals();
        }
    }

    /// <summary>
    /// True if base type of the input is an array
    /// </summary>
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

    /// <summary>
    /// Updates visibility and colors for the input elements and type icon
    /// </summary>
    private void UpdateInputsVisuals()
    {
        // inputs are not visible if it's not an input, but also invisible if it's an array since we don't provide a way to edit arrays via editor
        // because that would be too annoying to add 
        StringInput.Visible = InputType == VisLang.ValueType.String && IsManualInputVisible;
        NumberInput.Visible = InputType == VisLang.ValueType.Float && IsManualInputVisible;
        BoolInput.Visible = InputType == VisLang.ValueType.Bool && IsManualInputVisible;
        IntInput.Visible = InputType == VisLang.ValueType.Integer && IsManualInputVisible;
        if (Theme != null)
        {
            if (IsArray)
            {
                ArrayIcon.SelfModulate = TypeMatchingPermissions == FunctionInputInfo.TypePermissions.AllowAny ? Theme.AnyColor : Theme.GetColorForType(ArrayDataType ?? VisLang.ValueType.Array);
                ArrayIcon.Visible = true;
                DefaultIcon.Visible = false;
            }
            else
            {
                DefaultIcon.SelfModulate = TypeMatchingPermissions == FunctionInputInfo.TypePermissions.AllowAny ? Theme.AnyColor : Theme.GetColorForType(InputType);
                DefaultIcon.Visible = true;
                ArrayIcon.Visible = false;
            }
        }
        ErrorIcon.Visible = !Valid;
    }

    private void Pressed()
    {
        Selected?.Invoke(this);
    }

    /// <summary>
    /// Checks if connection to the given node is possible
    /// </summary>
    /// <param name="other">Node to check connections against</param>
    /// <returns>True if connection is valid</returns>
    public bool IsValidTypeConnection(NodeInput other)
    {
        bool validArrayConnection = true;
        // if both are arrays we have to check if they are compatible based on the stored type
        if (other.IsArray == IsArray)
        {
            // valid is 
            // both are arrays but accept any *
            // destination accepts any 
            // both have matching types *
            // * <- can be check by comparison of the nullable since it will success if either both null or both are not and have same data
            validArrayConnection = ArrayDataType == null || ArrayDataType == other.ArrayDataType;
        }
        // check if types are known at design time and if they are ensure that they match
        // if type can not be known at design time we allow the connection and make runtime deal with it
        // if input does not care what type is passed into we also allow the connection and make it runtime's job to deal with errors  
        return (other.InputType == InputType && validArrayConnection) ||
         (TypeMatchingPermissions == FunctionInputInfo.TypePermissions.AllowAny || other.TypeMatchingPermissions == FunctionInputInfo.TypePermissions.AllowAny);
    }

    /// <summary>
    /// Can this node connect to other node. Check should be called on destination and have source as the argument(call from right pass left)
    /// </summary>
    public bool CanConnect(NodeInput other)
    {
        // output can have as many connections as it wants, since it doesn't store any information about them
        bool connected = IsInput ? (Connection != null) : false;
        return Valid && !(connected || other.OwningNode == OwningNode || other.IsInput == IsInput || !IsValidTypeConnection(other));
    }

    /// <summary>
    /// Destroy this input and notify all listeners that it was destroyed
    /// </summary>
    public void Destroy()
    {
        Destroyed?.Invoke(this);
        QueueFree();
    }
}
