namespace VisLang.Editor;

using Godot;
using System;

/// <summary>
/// Main scene of the editor responsible for handling all of the functions and code
/// </summary>
public partial class ProjectEditorScene : Control
{
    [Export]
    public TabBar TabBar { get; private set; }

    [Export]
    public FunctionEditControl MainFunctionEditor { get; private set; }

    public void PrepareForExecution()
    {
        
    }
}
