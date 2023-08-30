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
    public VBoxContainer? ErrorMessageBox { get; private set; }
    [Export]
    public PackedScene? OutputMessagePrefab { get; private set; }
    [Export]
    public VBoxContainer? VariableDisplayContainer { get; private set; }


    private List<Label> _variableValues = new();

    private List<RichTextLabel> _outputMessages = new();
    private List<RichTextLabel> _errorMessages = new();

    private CodeExecutor? _codeExecutor;

    /// <summary>
    /// Prepares system for execution by conversion all visual nodes into nodes usable for execution, by collection data from all functions
    /// </summary>
    /// <returns>Data that can be used to execute use code</returns>
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

    /// <summary>
    /// Writes given message to the output box of the ui
    /// </summary>
    /// <param name="message">Message to write</param>
    private void PrintOutputMessage(string message)
    {
        GD.Print($"VisLang: {message}");
        RichTextLabel label = OutputMessagePrefab?.InstantiateOrNull<RichTextLabel>() ?? new RichTextLabel() { BbcodeEnabled = true };
        label.Text = message;
        _outputMessages.Add(label);
        OutputMessageBox?.AddChild(label);
    }

    /// <summary>
    /// Writes given message to the error output box of the ui
    /// </summary>
    /// <param name="message">Message to write</param>
    private void PrintErrorMessage(string message)
    {
        GD.Print($"VisLang: {message}");
        RichTextLabel label = OutputMessagePrefab?.InstantiateOrNull<RichTextLabel>() ?? new RichTextLabel() { BbcodeEnabled = true };
        label.Text = message;
        _errorMessages.Add(label);
        ErrorMessageBox?.AddChild(label);
    }

    /// <summary>
    /// Clears both message and error output of the editor removing all messages
    /// </summary>
    private void ClearOutputs()
    {
        foreach (RichTextLabel label in _outputMessages)
        {
            OutputMessageBox?.RemoveChild(label);
            label.QueueFree();
        }
        _outputMessages.Clear();
        foreach (RichTextLabel label in _errorMessages)
        {
            ErrorMessageBox?.RemoveChild(label);
            label.QueueFree();
        }
        _errorMessages.Clear();
    }

    /// <summary>
    /// Begin debug of the user code and put system into the execution mode. If this function is called when execution is paused it will cause execution to resume
    /// </summary>
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
        try
        {
            _codeExecutor.Run();
        }
        catch (Interpreter.InterpreterException e)
        {
            PrintErrorMessage(e.Message);
            HandleExecutionOver();
        }

        RuntimeControls.Visible = true;
    }

    /// <summary>
    /// Handle execution hitting a node that was marked as breakpoint. This will update variable display panel
    /// </summary>
    /// <param name="node">Editor node that was marked as breakpoint</param>
    /// <param name="sourceNode">Internal execution node</param>
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

    /// <summary>
    /// Handle execution completing. Just hides runtime controls
    /// </summary>
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
