namespace VisLang.Editor.Files;
using System.Collections.Generic;
using Godot;

public class EditorNodeSaveData
{
    public record NodeInputSaveData(object data, VisLang.ValueType type);

    public EditorNodeSaveData(string name, Vector2 position, string? functionInfoResourcePath)
    {
        Name = name;
        Position = position;
        FunctionInfoResourcePath = functionInfoResourcePath;
    }

    public List<NodeInputSaveData> ManualInputs { get; set; } = new();
    public string Name { get; set; }
    public Vector2 Position { get; set; }
    public string? FunctionInfoResourcePath { get; set; }
}