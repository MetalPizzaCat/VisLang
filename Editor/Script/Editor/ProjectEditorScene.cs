namespace VisLang.Editor;

using Godot;
using System.Linq;
using System.Collections.Generic;
using VisLang.Editor.Debug;

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
    public Control RuntimeControls { get; private set; }

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
    [Export]
    public VBoxContainer? VariableDisplayContainer { get; private set; }

    private List<Label> _variableValues = new();

    private List<RichTextLabel> _outputMessages = new();

    private CodeExecutor? _codeExecutor;

    public Debug.CodeExecutionData PrepareForExecution()
    {
        VisSystem system = new VisSystem();
        foreach (VariableInitInfo variable in MainFunctionEditor.VariableManager.GetVariableInits())
        {
            system.VisSystemMemory.CreateVariable(variable.Name, variable.Type);
        }
        system.Entrance = MainFunctionEditor.NodeCanvas.GenerateNodeTree(system);
        return new Debug.CodeExecutionData(system, MainFunctionEditor.NodeCanvas.BreakpointNodes.ToList());
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
        if (_codeExecutor != null && _codeExecutor.IsRunning && _codeExecutor.IsPaused)
        {
            _codeExecutor.Resume();
            return;
        }
        ClearOutputs();
        _codeExecutor = new CodeExecutor(PrepareForExecution());
        _codeExecutor.CodeExecutionData.System.OnOutputAdded += PrintOutputMessage;
        _codeExecutor.BreakpointHit += HandleBreakpointOnNode;
        _codeExecutor.ExecutionOver += HandleExecutionOver;
        _codeExecutor.Run();
        RuntimeControls.Visible = true;
    }

    private void HandleBreakpointOnNode(EditorGraphNode node, ExecutionNode sourceNode)
    {
        if (VariableDisplayContainer == null || _codeExecutor == null)
        {
            return;
        }
        _variableValues.ForEach(p =>
        {
            VariableDisplayContainer.RemoveChild(p);
            p.QueueFree();
        });
        _variableValues.Clear();
        foreach ((string name, uint address) in _codeExecutor.CodeExecutionData.System.VisSystemMemory.Variables)
        {
            VisLang.Value? value = _codeExecutor.CodeExecutionData.System.VisSystemMemory.GetValue(name, null);
            if (value == null)
            {
                continue;
            }
            Label label = new Label()
            {
                Text = $"({address}) {name} [{value.ValueType}] = {value?.Data}"
            };
            VariableDisplayContainer.AddChild(label);
            _variableValues.Add(label);
        }
    }

    private void HandleExecutionOver()
    {
        RuntimeControls.Visible = false;
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
