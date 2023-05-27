using Godot;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;

public partial class MainScene : Node2D
{

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

    [Export]
    public VariableManager? VariableManager { get; set; }

    [Export]
    public VBoxContainer? OutputTextList { get; set; }
    [Export]
    public PackedScene? MessageTextLabelPrefab { get; set; }
    private List<RichTextLabel> _outputMessages = new();
    [Export]
    public RuntimeVariableInformationControl? RuntimeVariableInformation { get; set; }
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
        TestNode.GenerateFunction(new FunctionInfo(false, new()
        {
            {"a", VisLang.ValueType.Number},
            {"b", VisLang.ValueType.Bool},
            {"banana", VisLang.ValueType.String}
        },
        VisLang.ValueType.Number, "VisLang.VariableSetNode"));

        TestNode2.GenerateFunction(new FunctionInfo(false, new()
        {
            {"Text", VisLang.ValueType.String}
        },
        VisLang.ValueType.Number, "VisLang.PrintNode"));
        _debugger.SystemOutput.CollectionChanged += OutputTextChanged;
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

    private void PrepareForExecution()
    {
        _debugger.InitNewSystem();
        if (_debugger.System == null)
        {
            return;
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
        //TODO: remove below code cause it's shit and was made for testing only
        VisLang.VisNode? node = TestNode.CreateNode(_debugger.System);
        if (node == null)
        {
            GD.PrintErr("Created node is not correct");
            return;
        }

        VisLang.VisNode? print = TestNode2.CreateNode(_debugger.System);
        if (print == null)
        {
            GD.PrintErr("Can't create print node");
            return;
        }
        (node as VisLang.VariableSetNode).Name = VariableManager.Variables[0].VariableName;
        (node as VisLang.ExecutionNode).DefaultNext = print as VisLang.ExecutionNode;
        _debugger.System.Entrance = (node as VisLang.VariableSetNode);
		_debugger.StartExecution();
    }

    private void Execute()
    {
        StopExecution();
        PrepareForExecution();
        _debugger.Execute();
        GD.Print(_debugger.System.VisSystemMemory[VariableManager.Variables[0].VariableName].Data);
    }
}
