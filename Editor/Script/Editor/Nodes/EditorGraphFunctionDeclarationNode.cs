using Godot;
using System;
using System.Collections.Generic;

namespace VisLang.Editor;
/// <summary>
/// Class that represents function declaration object <para/>
/// Instead of having separate structure for creating functions and to keep the simplicity of the language all user function info lives inside this node<para/>
/// Any change to function signature should be reflected in this node
/// </summary>
public partial class EditorGraphFunctionDeclarationNode : EditorGraphNode
{
    public string FunctionName
    {
        get => Title;
        set => Title = value;
    }

    /// <summary>
    /// All of the variables local to this function
    /// </summary>
    public List<VariableInitInfo> Variables { get; set; } = new();


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
        SetSlot(0, false, 0, new Color(0,0,0), true, ExecTypeId, new Color(0,0,0));
        int slotId = 1;
        foreach (FunctionInputInfo input in info.Inputs)
        {
            AddChild(new Label() { Text = input.InputName });
            SetSlot(slotId, false, 0, new Color(0,0,0), true, GetTypeIdForValueType(input.InputType), CodeTheme?.GetColorForType(input.InputType) ?? new Color());
            slotId++;
        }
    }
}