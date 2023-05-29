using System;
using VisLang;
using System.Collections.Generic;

/// <summary>
/// Information about a node for runtime node generation</para>
/// </summary>
public class FunctionInfo
{
    public string Name { get; set; } = "Invalid node name :3";

    /// <summary>
    /// If true a special executable node prefab will be spawned 
    /// </summary>
    public bool IsExecutable { get; set; } = false;

    /// <summary>
    /// Collection of all input types this node uses.
    /// Because of bug related to how Godot 4 handles C# dictionary(it just doesn't :3) FunctionInfo can not be made into Godot resource object
    /// </summary>
    public Dictionary<string, VisLang.ValueType> Inputs = new Dictionary<string, VisLang.ValueType>();

    public VisLang.ValueType? Output = null;

    /// <summary>
    /// Typename of the node for creating nodes at runtime via reflection.
    /// Type used here *must* have a constructor that takes no arguments
    /// </summary>
    public string NodeType { get; set; } = "VisLang.ExecutionNode";

    public FunctionInfo()
    {
    }

    public FunctionInfo(bool isExecutable, Dictionary<string, VisLang.ValueType> inputs, VisLang.ValueType? output, string nodeType, string nodeName = "Invalid node name :3")
    {
        IsExecutable = isExecutable;
        Inputs = inputs;
        Output = output;
        NodeType = nodeType;
        Name = nodeName;
    }
}