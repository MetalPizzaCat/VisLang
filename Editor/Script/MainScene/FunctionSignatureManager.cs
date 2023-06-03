using Godot;
using System;

public partial class FunctionSignatureManager : Node
{
    [Export]
    public Godot.Collections.Array<FunctionInfo> Functions { get; set; } = new();

    [Export]
    public Godot.Collections.Array<SpecialFunctionInfo> SpecialFunctions { get; set; } = new();
}
