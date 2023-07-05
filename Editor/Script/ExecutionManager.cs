namespace VisLang.Editor;
using Godot;
using System;

public partial class ExecutionManager : Node
{
	/// <summary>
	/// Editor scene from which all code should be read
	/// </summary>
    [Export]
    public ProjectEditorScene EditorScene { get; private set; }
}
