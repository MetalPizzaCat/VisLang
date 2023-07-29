namespace VisLang.Editor.Files;
using Godot;
using System.Collections.Generic;

public class NodeCollectionSaveData
{
    public Vector2 ScrollOffset { get; set; } = Vector2.Zero;
    public List<EditorNodeSaveData> GenericNodes { get; set; } = new();
    public List<VariableNodeSaveData> VariableNodes { get; set; } = new();
}