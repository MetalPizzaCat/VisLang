using System;
using VisLang;
using System.Collections.ObjectModel;

/// <summary>
/// Wrapper around basic VisLang system that allows step by step execution
/// </summary>
public class Debugger
{
    public delegate void ExecutionFinishedEventHandler();
    public event ExecutionFinishedEventHandler? ExecutionFinished;

    public ObservableCollection<string> SystemOutput { get; set; } = new();

    private VisSystem? _system = null;
    public VisSystem? System => _system;
    private ExecutionNode? _currentNode = null;

    private VisNode? _previousNode = null;

    /// <summary>
    /// Creates a new interpreter system and returns it
    /// </summary>
    /// <returns></returns>
    public VisSystem InitNewSystem()
    {
        _system = new VisSystem();
        // >:)
        _system.OnOutputAdded += (string output) => { SystemOutput.Add(output); };

        SystemOutput.Clear();
        return _system;
    }

    /// <summary>
    /// Begin execution
    /// </summary>
    public void StartExecution()
    {
        if (_system == null)
        {
            return;
        }
        _currentNode = _system.Entrance;
    }

    /// <summary>
    /// Moves to the next step of execution
    /// </summary>
    public void Step()
    {
        if(_previousNode?.DebugData is EditorVisNode old)
        {
            old.IsCurrentlyExecuted = false;
        }
        _currentNode?.Execute();
        if (_currentNode?.DebugData is EditorVisNode newNode)
        {
            newNode.IsCurrentlyExecuted = true;
        }
        _previousNode = _currentNode;
        _currentNode = _currentNode?.GetNext();
        if(_currentNode == null)
        {
            ExecutionFinished?.Invoke();
        }
    }

    public void Stop()
    {
        _system = null;
    }

    /// <summary>
    /// Runs whole system till the end
    /// </summary>
    public void Execute()
    {
        _system?.Execute();
    }
}