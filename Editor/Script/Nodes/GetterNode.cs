using Godot;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// A special execution node that does not need a FunctionInfo to be created and instead uses type of the variable
/// </summary>
public partial class GetterNode : VariableNode
{
    protected override FunctionInfo? GetFunctionInfo(VariableInfo val)
    {
        return new FunctionInfo
        (
            "Get",
            false,
            new(),
            "VisLang.VariableGetNode",
            true,
            val.ValueType,
            true,
            val.ArrayDataType != null,
            val.ArrayDataType ?? VisLang.ValueType.Bool
        );
    }

    protected override void ApplyAdditionalDataToNode<NodeType>(NodeType node)
    {
        base.ApplyAdditionalDataToNode(node);
        if (node is VisLang.VariableGetNode get)
        {
            get.Name = Info.Name;
        }
    }
}