using System;
using Godot;

/// <summary>
/// Special version of EditorVisNode that observes connections to a specific input and changes types of other specific nodes to have same type as the observed node
/// </summary>
public partial class ArrayEditorVisNode : EditorVisNode
{
    protected NodeInput ArrayInput;
    protected override void GenerateInputs(float inputNodeVisualOffset, FunctionInfo info)
    {
        // for nodes that depend on array type we manually generate additional input that will be the array input

        NodeInput? array = CreateInput(new FunctionInputInfo("Array", VisLang.ValueType.Array, null));
        if (array == null)
        {
            GD.PrintErr("Unable to create inputs from prefab");
            return;
        }
        array.ConnectionCreated += ArrayInputConnected;
        array.ConnectionDestroyed += ArrayInputDisconnected;
        Inputs.Add(array);
        NodeInputAnchor.AddChild(array);
        array.Position = new Vector2(0, inputNodeVisualOffset);
        ArrayInput = array;
        base.GenerateInputs(InputNodeOffsetStep + inputNodeVisualOffset, info);
    }

    private void ArrayInputConnected(NodeInput sender)
    {
        foreach (NodeInput input in Inputs)
        {
            if (input == ArrayInput || !input.IsArrayTypeDependent)
            {
                continue;
            }
            if (input.IsArray)
            {
                input.ArrayDataType = sender.ArrayDataType ?? VisLang.ValueType.Bool;
            }
            else
            {
                input.InputType = sender.ArrayDataType ?? VisLang.ValueType.Bool;
            }
        }
    }

    private void ArrayInputDisconnected(NodeInput sender)
    {

    }
}