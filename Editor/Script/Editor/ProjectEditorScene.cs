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

    [Export]
    public FileDialog? SaveFileDialog { get; private set; }

    [Export]
    public FileDialog? LoadFileDialog { get; private set; }

    [Export]
    public FileDialog ExportFileDialog { get; private set; }

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

    private void ClearOutputs()
    {
        foreach (RichTextLabel label in _outputMessages)
        {
            OutputMessageBox?.RemoveChild(label);
            label.QueueFree();
        }
        _outputMessages.Clear();
    }

    private void StartUserDebug()
    {
        ClearOutputs();
        VisSystem system = PrepareForExecution();
        system.OnOutputAdded += PrintOutputMessage;
        system.Execute();
    }

    private void Save()
    {
        SaveFileDialog?.Popup();
    }

    private void SaveCodeToFile(string file)
    {
        Files.ProjectSaveData data = new();
        data.Functions.Add("Main", MainFunctionEditor.GetSaveData());
        string code = Newtonsoft.Json.JsonConvert.SerializeObject(data);
        System.IO.File.WriteAllText(file, code);
    }

    private void LoadCodeFromFile(string file)
    {
        MainFunctionEditor.ClearCanvas();
        string code = System.IO.File.ReadAllText(file);
        Files.ProjectSaveData? data = Newtonsoft.Json.JsonConvert.DeserializeObject<Files.ProjectSaveData>(code);
        if (data == null)
        {
            GD.PrintErr($"Failed to load from {file}");
            return;
        }

        MainFunctionEditor.LoadSaveData(data.Functions["Main"]);
    }

    private void Load()
    {
        LoadFileDialog?.Popup();
    }
}
