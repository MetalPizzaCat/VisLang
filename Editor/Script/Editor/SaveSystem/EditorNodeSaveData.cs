namespace VisLang.Editor.Files;
using System;
using Godot;

public class EditorNodeSaveData
{
    public EditorNodeSaveData(Vector2 position, string? functionInfoResourcePath)
    {
        Position = position;
        FunctionInfoResourcePath = functionInfoResourcePath;
    }

    public Vector2 Position { get; set; }
    public string? FunctionInfoResourcePath { get; set; }
}