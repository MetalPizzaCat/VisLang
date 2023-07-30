namespace VisLang.Editor.Files;
using System;
using Godot;

public class EditorNodeSaveData
{
    public EditorNodeSaveData(string name, Vector2 position, string? functionInfoResourcePath)
    {
        Name = name;
        Position = position;
        FunctionInfoResourcePath = functionInfoResourcePath;
    }

    public string Name { get; set; }
    public Vector2 Position { get; set; }
    public string? FunctionInfoResourcePath { get; set; }
}