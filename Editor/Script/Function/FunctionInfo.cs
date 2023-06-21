using System;
using Godot;
using VisLang;
using System.Collections.Generic;
using Godot.Collections;

/// <summary>
/// Function info stores information about VisNodes that exist as native code
/// </summary>
[MonoCustomResourceRegistry.RegisteredType("FunctionInfo")]
public partial class FunctionInfo : Resource
{

    [Export]
    public string FunctionName { get; set; } = "Invalid node name :3";

    [Export(PropertyHint.MultilineText)]
    public string FunctionDescription {get;set;} = "Never gonna give you up";

    /// <summary>
    /// If true a special executable node prefab will be spawned 
    /// </summary>
    [Export]
    public bool IsExecutable { get; set; } = false;

    /// <summary>
    /// Information about all of the function arguments. Using additional class and Godot array due to a bug with c# dictionaries in godot(they don't work :3)
    /// </summary>
    [Export]
    public Godot.Collections.Array<FunctionInputInfo> Inputs = new();

    [Export]
    public VisLang.ValueType _output;

    [Export]
    public bool _isOutputArray;

    [Export]
    public bool HasOutput { get; set; } = false;

    public VisLang.ValueType? OutputType => HasOutput ? _output : null;
    public bool? IsOutputArray => HasOutput ? _isOutputArray : null;

    /// <summary>
    /// Typename of the node for creating nodes at runtime via reflection.
    /// Type used here *must* have a constructor that takes no arguments
    /// </summary>
    [Export]
    public string NodeType { get; set; } = "VisLang.ExecutionNode";

    public FunctionInfo()
    {
    }

    public FunctionInfo(string functionName, bool isExecutable, Array<FunctionInputInfo> inputs, string nodeType, bool hasOutput, VisLang.ValueType output = VisLang.ValueType.Bool, bool isOutArray = false)
    {
        FunctionName = functionName;
        IsExecutable = isExecutable;
        Inputs = inputs;
        _output = output;
        HasOutput = hasOutput;
        NodeType = nodeType;
        _isOutputArray = isOutArray;
    }
}