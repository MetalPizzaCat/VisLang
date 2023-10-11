using Godot;
using System;
using VisLang.Editor;

public partial class EditorGraphFunctionDeclarationNode : EditorGraphNode
{
    public string FunctionName
    {
        get => Title;
        set => Title = value;
    }

    public override void GenerateFunction(FunctionInfo info)
    {
        /*
        Provided function info only will store information about the function itself
        which means that it has arguments and return type if any
        But because this is declaration of the function we have to generate slightly differently
        1) All of the input arguments should become outputs on this node
        2) All outputs of this node should not be added
        */
        //base.GenerateFunction(new FunctionInfo() { FunctionName = "Function declaration test", IsExecutable = true });
        FunctionName = info.FunctionName;
        AddChild(new Label() { Text = "Exec" });
        SetSlot(0, false, 0, new Color(1, 1, 1), true, ExecTypeId, new Color(1, 1, 1));
        int slotId = 1;
        foreach (FunctionInputInfo input in info.Inputs)
        {
            AddChild(new Label() { Text = input.InputName });
            SetSlot(slotId, false, 0, new Color(1, 1, 1), true, GetTypeIdForValueType(input.InputType), CodeTheme?.GetColorForType(input.InputType) ?? new Color());
            slotId++;
        }
    }
}