using Godot;
using System;
using System.Collections.Generic;


public partial class EditorVisNode : Node2D
{
    public delegate void ExecNodeSelectedEventHandler(ExecInput node);
    public event ExecNodeSelectedEventHandler? ExecNodeSelected;

    public delegate void DeleteRequestedEventHandler(EditorVisNode node);
    public event DeleteRequestedEventHandler? DeleteRequested;

    public delegate void GrabbedEventHandler(EditorVisNode node);
    /// <summary>
    /// Invoked when user selects a node by pressing on it 
    /// </summary>
    public event GrabbedEventHandler? Grabbed;

    public delegate void ReleasedEventHandler(EditorVisNode node);
    /// <summary>
    /// Invoked when user stops pressing on a node
    /// </summary>
    public event ReleasedEventHandler? Released;

    public delegate void InputNodeSelectedEventHandler(NodeInput input);
    public event InputNodeSelectedEventHandler? InputNodeSelected;

    private FunctionInfo? _info = null;
    public FunctionInfo? FunctionInfo
    {
        get => _info;
        set
        {
            if (MainButton != null && value != null)
            {
                MainButton.TooltipText = value.FunctionDescription;
            }
            _info = value;
        }
    }

    [Export]
    public CodeColorTheme? CodeTheme { get; set; }
    [Export]
    public float InputNodeOffsetStep { get; set; } = 32f;

    [Export]
    public Button? MainButton { get; set; }
    [Export]
    public PackedScene? NodeInputPrefab { get; set; } = null;

    [Export]
    public Node2D NodeInputAnchor { get; set; }

    [Export]
    public NodeInput NodeOutput { get; set; }

    [Export]
    public Label? NodeNameLabel { get; set; }
    [Export]
    public PopupMenu? ContextMenu { get; set; }

    [ExportGroup("Exec")]
    [Export]
    public ExecInput? InputExecNode { get; set; }
    [Export]
    public ExecInput? OutputExecNode { get; set; }
    [ExportGroup("User debug")]
    [Export]
    public Sprite2D? ExecutionDebugIcon { get; set; }

    public List<NodeInput> Inputs { get; set; } = new List<NodeInput>();
    /// <summary>
    /// All of the inputs that had connections before node was recreated 
    /// </summary>
    protected List<NodeInput> InvalidInputs { get; set; } = new();


    private bool _isCurrentlyExecuted = false;
    public bool IsCurrentlyExecuted
    {
        get => _isCurrentlyExecuted;
        set
        {
            _isCurrentlyExecuted = value;
            if (ExecutionDebugIcon != null)
            {
                ExecutionDebugIcon.Visible = value;
            }
        }
    }
    public override void _Ready()
    {
        if (FunctionInfo != null)
        {
            GenerateFunction(FunctionInfo);
        }
        if (InputExecNode != null)
        {
            InputExecNode.Selected += InputExecSelected;
            InputExecNode.OwningNode = this;
        }
        if (OutputExecNode != null)
        {
            OutputExecNode.Selected += OutputExecSelected;
            OutputExecNode.OwningNode = this;
        }
        NodeOutput.Selected += (NodeInput input) => { InputNodeSelected?.Invoke(input); };
        NodeOutput.OwningNode = this;
    }

    /// <summary>
    /// Call this when node is added to the canvas for the node to perform all of the additional event connections and node adjustments
    /// </summary>
    /// <param name="canvas">Main canvas</param>
    public virtual void InitOnCanvas(MainScene canvas)
    {
        // grab any additional events here
    }

    protected NodeInput? CreateInput(FunctionInputInfo argument)
    {
        NodeInput? input = NodeInputPrefab?.InstantiateOrNull<NodeInput>();
        if (input == null)
        {
            GD.PrintErr("Unable to create inputs from prefab");
            return null;
        }
        input.OwningNode = this;
        input.InputName = argument.InputName;
        input.TypeMatchingPermissions = argument.TypeMatchingPermissions;
        input.IsArrayTypeDependent = argument.IsArrayTypeDependent;
        input.InputType = argument.InputType;
        input.ArrayDataType = argument.HasArrayType ? argument.ArrayDataType : null;
        input.Selected += (NodeInput input) => { InputNodeSelected?.Invoke(input); };

        return input;
    }

    /// <summary>
    /// Generates and adds new inputs to the inputs array. Previous inputs are not erased
    /// </summary>
    /// <param name="inputNodeVisualOffset">Start offset for the nodes</param>
    protected virtual void GenerateInputs(float inputNodeVisualOffset, FunctionInfo info)
    {
        float currentInputOffset = inputNodeVisualOffset;
        foreach (FunctionInputInfo argument in info.Inputs)
        {
            NodeInput? input = CreateInput(argument);
            if (input == null)
            {
                GD.PrintErr("Unable to create inputs from prefab");
                return;
            }
            Inputs.Add(input);
            NodeInputAnchor.AddChild(input);
            input.Position = new Vector2(0, currentInputOffset);
            currentInputOffset += InputNodeOffsetStep;
        }
    }

    protected virtual void GenerateOutput(FunctionInfo info)
    {
        NodeOutput.Visible = true;
        NodeOutput.InputType = info.OutputType ?? VisLang.ValueType.Bool;
        NodeOutput.ArrayDataType = info.HasOutputTypeArrayType ? info.OutputArrayType : null;
        NodeOutput.TypeMatchingPermissions = (info.IsOutputTypeKnown ?? false) ? FunctionInputInfo.TypePermissions.SameTypeOnly : FunctionInputInfo.TypePermissions.AllowAny;
    }

    public void GenerateFunction(FunctionInfo info)
    {
        FunctionInfo = info;
        if (NodeInputPrefab == null)
        {
            throw new NullReferenceException("Unable to create node from description because input node was not provided");
        }

        // since some nodes might have connections and since i don't want to just delete the connections 
        // as that would create some confusion(for example if user doesn't not notice connections disappearing)
        // to combat that we will keep nodes that have connections intact but move them to the bottom of the list
        // ***
        // there is a bit of an issue here for when user might change type and then change it back since in this case 
        // the connection will still be marked as invalid
        foreach (NodeInput input in Inputs)
        {
            if (input.IsInputConnected)
            {
                InvalidInputs.Add(input);
                // this way once user tries to reset connection node will disappear, preventing further confusion
                input.ConnectionDestroyed += DestroyInvalidNode;
            }
            else
            {
                input.QueueFree();
            }
        }
        Inputs.Clear();

        GenerateInputs(0f, info);

        float offset = Inputs.Count * InputNodeOffsetStep + InputNodeOffsetStep;
        foreach (NodeInput input in InvalidInputs)
        {
            input.Position = new Vector2(0, offset);
            offset += InputNodeOffsetStep;
            input.Valid = false;
        }

        if (info.HasOutput)
        {
            GenerateOutput(info);
        }
        if (NodeNameLabel != null)
        {
            NodeNameLabel.Text = info.FunctionName;
        }
    }

    private void DestroyInvalidNode(NodeInput input)
    {
        InvalidInputs.Remove(input);
        input.Destroy();
    }

    protected virtual void ApplyAdditionalDataToNode<NodeType>(NodeType node) where NodeType : VisLang.VisNode
    {
        // implement in this child to set whatever extra data you want :3
        // for example SetterNode should implement to have variable name setting here
    }

    public NodeType? CreateNode<NodeType>(VisLang.VisSystem? interpreter) where NodeType : VisLang.VisNode
    {
        if (FunctionInfo == null)
        {
            throw new MissingFunctionInfoException("Attempted to create a function but FunctionInfo is null");
        }
        NodeType? node = (NodeType?)Activator.CreateInstance("VisLang", FunctionInfo.NodeType)?.Unwrap();
        if (node == null)
        {
            return null;
        }
        node.Interpreter = interpreter;
        ApplyAdditionalDataToNode(node);
        return node;
    }

    public void InputExecSelected(ExecInput exec)
    {
        ExecNodeSelected?.Invoke(exec);
    }

    public void OutputExecSelected(ExecInput exec)
    {
        ExecNodeSelected?.Invoke(exec);
    }

    private void GrabNode()
    {
        Grabbed?.Invoke(this);
    }

    private void ReleaseNode()
    {
        Released?.Invoke(this);
    }

    protected virtual void ContextMenuOptionSelected(long option)
    {
        if (option == 0)
        {
            DeleteRequested?.Invoke(this);
        }
    }

    protected void ButtonGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton btn && btn.Pressed && btn.ButtonIndex == MouseButton.Right && ContextMenu != null)
        {
            ContextMenu.Position = new Vector2I((int)btn.Position.X, (int)btn.Position.Y);
            ContextMenu?.Popup();
        }
    }
}
