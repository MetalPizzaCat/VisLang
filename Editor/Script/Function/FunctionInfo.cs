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
    public string FunctionDescription { get; set; } = "Never gonna give you up";

    /// <summary>
    /// If true a special executable node prefab will be spawned 
    /// </summary>
    [Export]
    public bool IsExecutable { get; set; } = false;

    /// <summary>
    /// If true all inputs marked as "ArrayTypeDependent" will have have their value type changed based on the type of the connected array
    /// </summary>
    [Export]
    public bool IsArrayTypeDependent { get; set; } = false;
    /// <summary>
    /// If IsArrayTypeDependent is set to true, connections to port that handles argument number ArrayTypeDefiningArgumentId 
    /// will cause changes of to the node type
    /// </summary>
    [Export]
    public int ArrayTypeDefiningArgumentId { get; set; } = 0;

    /// <summary>
    /// Information about all of the function arguments. Using additional class and Godot array due to a bug with c# dictionaries in godot(they don't work :3)
    /// </summary>
    [Export]
    public Godot.Collections.Array<FunctionInputInfo> Inputs { get; set; } = new();

    [ExportGroup("Output")]
    [Export]
    private VisLang.ValueType _output;
    [Export]
    public bool IsOutputArrayTypeDependent { get; set; } = false;

    /// <summary>
    /// If true output type is know and can be used during preprocessing stage checks
    /// </summary>
    [Export]
    private bool _isOutputTypeKnown = true;

    [Export]
    public bool HasOutput { get; set; } = false;
    [ExportSubgroup("Array")]
    [Export]
    private bool _hasOutputArrayType = false;
    [Export]
    public VisLang.ValueType _outputArrayType = VisLang.ValueType.Bool;


    public VisLang.ValueType? OutputType => HasOutput ? _output : null;
    public bool? IsOutputTypeKnown => HasOutput ? _isOutputTypeKnown : null;
    public bool HasOutputTypeArrayType => _hasOutputArrayType;
    public VisLang.ValueType? OutputArrayType => HasOutput && _hasOutputArrayType ? _outputArrayType : null;

    [ExportGroup("Execution")]
    /// <summary>
    /// Typename of the node for creating nodes at runtime via reflection.
    /// Type used here *must* have a constructor that takes no arguments
    /// </summary>
    [Export]
    public string NodeType { get; set; } = "VisLang.ExecutionNode";

    public FunctionInfo()
    {
    }

    public FunctionInfo
    (
        string functionName,
        bool isExecutable,
        Array<FunctionInputInfo> inputs,
        string nodeType,
        bool hasOutput,
        VisLang.ValueType output = VisLang.ValueType.Bool,
        bool outputTypeKnown = true,
        bool hasOutputTypeArrayType = false,
        VisLang.ValueType outputArrayType = VisLang.ValueType.Bool
    )
    {
        FunctionName = functionName;
        IsExecutable = isExecutable;
        Inputs = inputs;
        _output = output;
        HasOutput = hasOutput;
        NodeType = nodeType;
        _isOutputTypeKnown = outputTypeKnown;
        _hasOutputArrayType = hasOutputTypeArrayType;
        _outputArrayType = outputArrayType;
    }
}