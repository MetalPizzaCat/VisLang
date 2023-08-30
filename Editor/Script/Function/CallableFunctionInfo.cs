using Godot;
using System;

namespace VisLang.Editor.CallableEditor;

/// <summary>
/// Stores full information about a callable object that user created
/// </summary>
public class CallableFunctionInfo
{
    public Guid Guid { get; set; }
    public FunctionInfo Info { get; set; }
}