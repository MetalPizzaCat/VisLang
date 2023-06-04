using System;
using Godot;

/// <summary>
/// Function info for special nodes like branch,get,set,etc.
/// </summary>
[MonoCustomResourceRegistry.RegisteredType("SpecialFunctionInfo")]
public partial class SpecialFunctionInfo : Resource
{
    [Export]
    public string FunctionName { get; set; } = "Default";

    [Export(PropertyHint.MultilineText)]
    public string FunctionDescription { get; set; } = "Some smart joke here";

    [Export]
    public PackedScene? Prefab { get; set; }
}