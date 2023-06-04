using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.IO;

public partial class MainScene : Node2D
{

    [Export]
    public VariableManager? VariableManager { get; set; }
    [Export]
    public FunctionSignatureManager? FunctionSignatureManager { get; set; }

    [Export]
    public VBoxContainer? OutputTextList { get; set; }
    [Export]
    public PackedScene? MessageTextLabelPrefab { get; set; }
    private List<RichTextLabel> _outputMessages = new();
    [Export]
    public RuntimeVariableInformationControl? RuntimeVariableInformation { get; set; }

    /// <summary>
    /// Node connected to this input will be the first node to execute</para>
    /// Not having this as proper node will mean no code will be able to execute
    /// </summary>
    [Export]
    public ExecInput EntranceInput { get; set; }

    [Export]
    public ConnectionManager ConnectionManager { get; set; }
    [Export]
    public NodeMovementManager MovementManager { get; set; }

    [Export]
    public NodeCreationMenu NodeCreationMenu { get; set; }

    public List<VisNode> Nodes { get; set; } = new();

    [ExportGroup("Runtime creation")]
    [Export]
    public CanvasControl? CanvasControl { get; set; }
    [ExportSubgroup("Prefabs")]
    [Export]
    public PackedScene? VisNodePrefab { get; set; }
    [Export]
    public PackedScene? ExecNodePrefab { get; set; }


    public Vector2 MouseLocation => (CanvasControl?.MousePosition - (CanvasControl?.DefaultViewportSize ?? Vector2.Zero) / 2f) ?? Vector2.Zero;

    [ExportGroup("Editor helper")]
    [Export]
    public FileDialog? TemplateSaveDialog { get; set; }

    private Debugger _debugger = new Debugger();
    private bool _isExecuting = false;
    public bool IsExecuting
    {
        get => _isExecuting;
        set
        {
            _isExecuting = value;
            if (RuntimeVariableInformation != null)
            {
                RuntimeVariableInformation.Visible = value;
            }
        }
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _debugger.SystemOutput.CollectionChanged += OutputTextChanged;
        EntranceInput.Selected += ExecConnectionSelected;
        NodeCreationMenu.FunctionSelected += CreateNode;
        NodeCreationMenu.SpecialFunctionSelected += CreateSpecialNode;
        if (VariableManager != null)
        {
            VariableManager.GetterRequested += CreateGetter;
            VariableManager.SetterRequested += CreateSetter;
        }
    }

    private void CreateGetter(VariableInfo info)
    {
        VariableNode? node = FunctionSignatureManager?.GetterPrefab?.InstantiateOrNull<VariableNode>();
        if (node == null)
        {
            GD.PrintErr($"Failed to create getter for {info.Name} of {info.ValueType}");
            return;
        }
        node.InitControl(info);
        InitNode(node, false);
    }

    private void CreateSetter(VariableInfo info)
    {
        VariableNode? node = FunctionSignatureManager?.SetterPrefab?.InstantiateOrNull<VariableNode>();
        if (node == null)
        {
            GD.PrintErr($"Failed to create setter for {info.Name} of {info.ValueType}");
            return;
        }
        node.InitControl(info);
        InitNode(node, false);
    }

    private void CreateSpecialNode(SpecialFunctionInfo info)
    {
        VisNode? node = info.Prefab?.InstantiateOrNull<VisNode>();
        if (node == null)
        {
            GD.PrintErr($"Failed to create node for {info.FunctionName}");
            return;
        }
        InitNode(node);
    }

    /// <summary>
    /// Creates node at mouse location
    /// </summary>
    /// <param name="info">Function signature</param>
    private void CreateNode(FunctionInfo info)
    {
        VisNode? node = null;
        if (info.IsExecutable)
        {
            node = ExecNodePrefab?.InstantiateOrNull<VisNode>();
        }
        else
        {
            node = VisNodePrefab?.InstantiateOrNull<VisNode>();
        }

        if (node == null)
        {
            return;
        }

        InitNode(node);
        node.GenerateFunction(info);
    }

    private void InitNode(VisNode node, bool useMouseLocation = true)
    {
        node.ExecNodeSelected += ExecConnectionSelected;
        node.Grabbed += NodeGrabbed;
        node.Released += NodeReleased;
        node.InputNodeSelected += InputConnectionSelected;

        node.GlobalPosition = useMouseLocation ? MouseLocation : Vector2.Zero;
        AddChild(node);
        Nodes.Add(node);
        node.InitOnCanvas(this);
    }

    private void InputConnectionSelected(NodeInput input)
    {
        ConnectionManager.SelectInputConnector(input);
    }
    private void ExecConnectionSelected(ExecInput input)
    {
        ConnectionManager.SelectExecConnector(input);
    }

    private void OutputTextChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (OutputTextList != null && e.NewItems != null)
                {
                    foreach (object? item in e.NewItems)
                    {
                        if (item == null)
                        {
                            AddOutput("Invalid message logged", extraBad: true);
                        }
                        else
                        {
                            AddOutput(item as string);
                        }
                    }
                }
                break;
        }
    }

    private void AddOutput(string output, bool warning = false, bool extraBad = false)
    {
        if (OutputTextList == null)
        {
            GD.PrintErr("Log print requested but no element for text output was provided");
            return;
        }
        RichTextLabel label = MessageTextLabelPrefab?.InstantiateOrNull<RichTextLabel>() ?? new RichTextLabel() { BbcodeEnabled = true };
        label.Text = extraBad ? $"[color=red]{output}[/color]" : output;
        _outputMessages.Add(label);
        OutputTextList.AddChild(label);
    }

    private void StopExecution()
    {
        IsExecuting = false;
        _debugger.Stop();
    }

    private void StepExecution()
    {
        if (_debugger.System == null)
        {
            PrepareForExecution();
            foreach (RichTextLabel label in _outputMessages)
            {
                OutputTextList?.RemoveChild(label);
                label.QueueFree();
            }
            _outputMessages.Clear();
            IsExecuting = true;
        }

        _debugger.Step();
        RuntimeVariableInformation?.DisplayInformation(_debugger.System.VisSystemMemory);
    }

    private bool PrepareForExecution()
    {
        _debugger.InitNewSystem();
        if (_debugger.System == null)
        {
            GD.PrintErr("Failed to initialize debug system");
            return false;
        }
        if (VariableManager != null)
        {
            foreach (VariableControl va in VariableManager.VariableControlButtons)
            {
                if (string.IsNullOrWhiteSpace(va.VariableName))
                {
                    throw new Exception("Bad variable name!");
                }
                _debugger.System.VisSystemMemory.CreateVariable(va.VariableName, va.VariableType);
            }
        }
        if (EntranceInput.Connection == null || EntranceInput.Connection.OwningNode == null)
        {
            return false;
        }
        NodeParser parser = new NodeParser(EntranceInput.Connection.OwningNode, _debugger.System);
        VisLang.VisNode? root = parser.Parse();
        if (root == null)
        {
            GD.PrintErr("No root node is available");
            return false;
        }
        _debugger.System.Entrance = (root as VisLang.ExecutionNode);
        _debugger.StartExecution();
        return true;
    }

    private void Execute()
    {
        StopExecution();
        PrepareForExecution();
        _debugger.Execute();
    }

    private void NodeGrabbed(VisNode node)
    {
        MovementManager.SelectedNode = node;
    }

    private void NodeReleased(VisNode node)
    {
        if (MovementManager.SelectedNode == node)
        {
            MovementManager.SelectedNode = null;
        }
    }
}
