using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class FunctionSignatureManager : Node
{
    [Export]
    public Godot.Collections.Array<FunctionInfo> Functions { get; set; } = new();

    [Export]
    public FunctionManagementControl UserFunctions { get; set; }

    public IEnumerable<FunctionInfo> UserFunctionSignatures => UserFunctions.CallableFunctions.Select(p => p.Info);
}
