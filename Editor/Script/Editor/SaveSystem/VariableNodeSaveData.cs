namespace VisLang.Editor.Files;
using System;
using Godot;
using System.Collections.Generic;

public class VariableNodeSaveData : EditorNodeSaveData
{
    public VariableNodeSaveData(Guid variableId, bool isGetter, Vector2 position) : base(position, functionInfoResourcePath : null)
    {
        VariableId = variableId;
        IsGetter = isGetter;
    }

    public Guid VariableId { get; set; }
    public bool IsGetter { get; set; }
}