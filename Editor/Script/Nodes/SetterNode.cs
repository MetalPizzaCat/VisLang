using Godot;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// A special execution node that does not need a FunctionInfo to be created and instead uses type of the variable
/// </summary>
public partial class SetterNode : VariableNode
{
    protected override FunctionInfo? GetFunctionInfo(VariableInfo val)
    {
        return new FunctionInfo("Set", true, new Godot.Collections.Array<FunctionInputInfo>()
        {
             new FunctionInputInfo("Value", val.ValueType, val.ArrayDataType)
        }, "VisLang.VariableSetNode", false);
    }

    protected override void ApplyAdditionalDataToNode<NodeType>(NodeType node)
    {
        base.ApplyAdditionalDataToNode(node);
        if (node is VisLang.VariableSetNode set)
        {
            set.Name = Info.Name;
        }
    }
}