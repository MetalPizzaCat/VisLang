using Godot;
using System;
using System.Collections.Generic;


public partial class VisNode : Node2D
{
    public delegate void ExecNodeSelectedEventHandler(ExecInput node);
    public event ExecNodeSelectedEventHandler? ExecNodeSelected;

    public delegate void GrabbedEventHandler(VisNode node);
    /// <summary>
    /// Invoked when user selects a node by pressing on it 
    /// </summary>
    public event GrabbedEventHandler? Grabbed;

    public delegate void ReleasedEventHandler(VisNode node);
    /// <summary>
    /// Invoked when user stops pressing on a node
    /// </summary>
    public event ReleasedEventHandler? Released;

    public FunctionInfo? FunctionInfo { get; set; } = null;

    [Export]
    public PackedScene? NodeInputPrefab { get; set; } = null;

    [Export]
    public Node2D NodeInputAnchor { get; set; }

    [Export]
    public Label? NodeNameLabel { get; set; }

    [ExportGroup("Exec")]
    [Export]
    public ExecInput? InputExecNode { get; set; }
    [Export]
    public ExecInput? OutputExecNode { get; set; }

    public List<NodeInput> Inputs { get; set; } = new List<NodeInput>();

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
    }

    public void GenerateFunction(FunctionInfo info)
    {
        FunctionInfo = info;
        if (NodeInputPrefab == null)
        {
            throw new NullReferenceException("Unable to create node from description because input node was not provided");
        }


        foreach (NodeInput input in Inputs)
        {
            input.QueueFree();
        }
        Inputs.Clear();


        float currentInputOffset = 0;
        foreach ((string name, VisLang.ValueType arg) in info.Inputs)
        {
            NodeInput input = NodeInputPrefab.InstantiateOrNull<NodeInput>();
            input.InputName = name;
            input.InputType = arg;
            Inputs.Add(input);
            NodeInputAnchor.AddChild(input);
            input.Position = new Vector2(0, currentInputOffset);
            //TODO: make this constant dynamic to avoid recompiling code each time you feel like making ui pretty
            currentInputOffset += 32f;
        }
        if (NodeNameLabel != null)
        {
            NodeNameLabel.Text = info.Name;
        }
    }

    protected virtual void ApplyAdditionalDataToNode<NodeType>(NodeType node) where NodeType : VisLang.VisNode
    {
        // implement in this child to set whatever extra data you want :3
        // for example SetterNode should implement to have variable name setting here
    }

    public NodeType? CreateNode<NodeType>(VisLang.VisSystem? interpreter) where NodeType : VisLang.VisNode
    {
        NodeType? node = (NodeType?)Activator.CreateInstance("VisLang", FunctionInfo.NodeType)?.Unwrap();
        if (node == null)
        {
            return null;
        }
        node.Interpreter = interpreter;
        foreach (NodeInput input in Inputs)
        {
            // TODO: Add connection check once node connection will be implemented
            node?.Inputs.Add(new VisLang.VariableGetConstNode() { Value = new VisLang.Value(input.InputType, false, input.Data) });
        }
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
}
