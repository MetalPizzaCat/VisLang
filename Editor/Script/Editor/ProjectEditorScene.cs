namespace VisLang.Editor;

using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Main scene of the editor responsible for handling all of the functions and code
/// </summary>
public partial class ProjectEditorScene : Control
{
    [Export]
    public TabBar TabBar { get; private set; }

    [Export]
    public FunctionEditControl MainFunctionEditor { get; private set; }

    [ExportGroup("User code debug")]
    [Export]
    public VBoxContainer? OutputMessageBox { get; private set; }
    [Export]
    public PackedScene? OutputMessagePrefab { get; private set; }

    private List<RichTextLabel> _outputMessages = new();

    public VisSystem PrepareForExecution()
    {
        VisSystem system = new VisSystem();
        foreach (VariableInitInfo variable in MainFunctionEditor.VariableManager.GetVariableInits())
        {
            system.VisSystemMemory.CreateVariable(variable.Name, variable.Type);
        }
        system.Entrance = MainFunctionEditor.NodeCanvas.GenerateNodeTree(system);
        return system;
    }

    private void PrintOutputMessage(string message)
    {
        GD.Print($"VisLang: {message}");
        RichTextLabel label = OutputMessagePrefab?.InstantiateOrNull<RichTextLabel>() ?? new RichTextLabel() { BbcodeEnabled = true };
        label.Text = message;
        _outputMessages.Add(label);
        OutputMessageBox?.AddChild(label);
    }

    private void StartUserDebug()
    {
        GD.Print("BUTTON!");
        VisSystem system = PrepareForExecution();
        system.OnOutputAdded += PrintOutputMessage;
        system.Execute();
    }
}
