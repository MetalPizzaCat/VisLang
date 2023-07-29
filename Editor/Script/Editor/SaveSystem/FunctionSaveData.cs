namespace VisLang.Editor.Files;
using System;
using Godot;
using System.Collections.Generic;

public class FunctionSaveData
{
    public FunctionSaveData()
    {
    }

    public List<VariableInitInfo> Variables { get; set; } = new();

    public NodeCollectionSaveData Nodes { get; set; } = new();
}