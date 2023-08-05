namespace VisLang.Editor.Files;
using System;
using Godot;
using System.Collections.Generic;

public class VariableNodeSaveData : EditorNodeSaveData
{
    public VariableNodeSaveData(Guid variableId, bool isGetter, string name, Vector2 position) : base(name, position, functionInfoResourcePath: null)
    {
        VariableId = variableId;
        IsGetter = isGetter;
    }

    public Guid VariableId { get; set; }
    public bool IsGetter { get; set; }
}