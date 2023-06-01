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

    [ExportGroup("Debug")]
    /// <summary>
    /// Node meant for testing
    /// </summary>
    /// <value></value>
    [Export]
    public VisNode TestNode { get; set; }

    /// <summary>
    /// Node also meant for testing(this time for printing)
    /// </summary>
    /// <value></value>
    [Export]
    public VisNode TestNode2 { get; set; }

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
        TestNode.GenerateFunction(new FunctionInfo("Set", false, new()
        {
            new FunctionInputInfo("a", VisLang.ValueType.Number),
            new FunctionInputInfo("b", VisLang.ValueType.Bool),
            new FunctionInputInfo("banana", VisLang.ValueType.String)
        },
       "VisLang.VariableSetNode", false));

        VariableManager.VariableListChanged += (TestNode as SetterNode).VariableListUpdated;
        TestNode.ExecNodeSelected += ExecConnectionSelected;

        TestNode2.GenerateFunction(new FunctionInfo("Print", false, new()
        {
            new FunctionInputInfo("Text", VisLang.ValueType.String)
        },
        "VisLang.PrintNode", false));

        TestNode2.ExecNodeSelected += ExecConnectionSelected;
        EntranceInput.Selected += ExecConnectionSelected;

        TestNode.Grabbed += NodeGrabbed;
        TestNode.Released += NodeReleased;
        TestNode2.Grabbed += NodeGrabbed;
        TestNode2.Released += NodeReleased;

        _debugger.SystemOutput.CollectionChanged += OutputTextChanged;
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

    /// <summary>
    /// This simply traverses the line from root node and till the last available node. This does not generate data nodes
    /// </summary>
    /// <param name="root">Start editor node</param>
    /// <param name="interpreter">Interpreter system itself</param>
    /// <returns>Resulting root node with child nodes attached or null if generation failed</returns>
    private VisLang.VisNode? GenerateExecutionTree(VisNode root, VisLang.VisSystem interpreter)
    {
        VisLang.ExecutionNode? node = root.CreateNode<VisLang.ExecutionNode>(interpreter);
        if (node == null)
        {
            return null;
        }

        if (root.OutputExecNode != null && root.OutputExecNode.Connection != null && root.OutputExecNode.Connection.OwningNode != null)
        {
            VisLang.VisNode? next = GenerateExecutionTree(root.OutputExecNode.Connection.OwningNode, interpreter);
            if (next != null)
            {
                // any node that is connected to exec line is by definition an exec node
                // right????
                // TODO: Actually check if i'm right >_>
                node.DefaultNext = (next as VisLang.ExecutionNode);
            }
        }
        return node;
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
            foreach (VariableControl va in VariableManager.Variables)
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
        VisLang.VisNode? root = GenerateExecutionTree(EntranceInput.Connection.OwningNode, _debugger.System);
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
        GD.Print(_debugger.System.VisSystemMemory[VariableManager.Variables[0].VariableName].Data);
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

    private void GenerateTemplate()
    {
        TemplateSaveDialog?.Show();
    }

    private void SaveFunctionInfoTemplate(string path)
    {
        FunctionInfo info = new FunctionInfo();
        File.WriteAllText(path, Newtonsoft.Json.JsonConvert.SerializeObject(info));
    }
}
