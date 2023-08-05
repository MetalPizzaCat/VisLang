namespace VisLang.Editor.Files;
using System.Collections.Generic;
using Godot;

public class EditorNodeSaveData
{
    public record NodeInputSaveData(object Data, VisLang.ValueType Type);

    public EditorNodeSaveData(string name, Vector2 position, string? functionInfoResourcePath)
    {
        Name = name;
        Position = position;
        FunctionInfoResourcePath = functionInfoResourcePath;
    }

    /// <summary>
    /// Values of all objects that have manual input fields
    /// </summary>
    public List<NodeInputSaveData> ManualInputs { get; set; } = new();

    /// <summary>
    /// Names of all the input ports that store invalid connections
    /// </summary>
    public List<string> InvalidInputs { get; set; } = new List<string>();

    public bool HasInvalidOutput { get; set; } = false;

    public string Name { get; set; }
    public Vector2 Position { get; set; }
    public string? FunctionInfoResourcePath { get; set; }
}