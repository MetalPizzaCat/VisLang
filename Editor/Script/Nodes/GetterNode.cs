using Godot;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// A special execution node that does not need a FunctionInfo to be created and instead uses type of the variable
/// </summary>
public partial class GetterNode : VariableNode
{
    protected override FunctionInfo? GetFunctionInfo(Variable val)
    {
        return new FunctionInfo("Get", false, new(), "VisLang.VariableGetNode", true, val.ValueType);
    }

    protected override void ApplyAdditionalDataToNode<NodeType>(NodeType node)
    {
        base.ApplyAdditionalDataToNode(node);
        if (node is VisLang.VariableGetNode get && CurrentVariable != null)
        {
            get.Name = CurrentVariable.Name;
        }
    }
}