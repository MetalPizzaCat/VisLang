namespace VisLang.Editor.Files;

using Godot;
using System;
using System.Collections.Generic;

public class ProjectSaveData
{
    public ProjectSaveData()
    {
    }

    public string Name { get; set; } = "Default :3";

    public Dictionary<string, FunctionSaveData> Functions { get; set; } = new();
}