using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace VisLang;

/// <summary>
/// Class responsible for storing and handling all of the information related to language execution
/// </summary>
public class VisSystem
{
    public delegate void OutputAddedEventHandler(string output);
    public event OutputAddedEventHandler? OnOutputAdded;

    public List<ExecutionNode> Code { get; set; } = new();

    public VisSystemMemory VisSystemMemory { get; set; } = new();

    /// <summary>
    /// All of the procedures loaded in the memory that can be referenced by nodes inside this system
    /// </summary>
    /// <returns></returns>
    public List<VisProcedure> Procedures { get; set; } = new();

    public VisProcedure? GetProcedure(string name) => Procedures.FirstOrDefault(p => p.Name == name);

    public List<VisFunction> Functions { get; set; } = new();

    public VisFunction? GetFunction(string name) => Functions.FirstOrDefault(p => p.Name == name);

    /// <summary>
    /// Output produced during the execution by Execution nodes
    /// </summary>
    /// <returns></returns>
    public List<string> Output { get; set; } = new();

    /// <summary>
    /// Node from which the execution starts
    /// </summary>
    /// <value></value>
    public ExecutionNode? Entrance { get; set; } = null;

    public ExecutionNode? Current { get; set; } = null;

    public void AddOutput(string output)
    {
        OnOutputAdded?.Invoke(output);
        Output.Add(output);
    }

    /// <summary>
    /// Execute next item in the tree
    /// </summary>
    /// <returns>True if there is a next node that can be executed or false if no further nodes available</returns>
    public bool ExecuteNext(NodeContext? context)
    {
        if (Current == null)
        {
            return false;
        }
        Current.Execute(context);
        // second check here because some nodes might alter value of the Current(for example nodes that jump to a different set of instructions )
        if (Current == null)
        {
            return false;
        }
        Current = Current.GetNext();
        return Current != null;
    }

    /// <summary>
    /// Begins the execution from the first node and continues as long as there are nodes in the list
    /// </summary>
    public void Execute()
    {
        Entrance?.Execute();
        Current = Entrance?.GetNext();
        while (ExecuteNext(null) && Current != null)
        {
            Current.Execute();
            Current = Current.GetNext();
        }
    }

    /// <summary>
    /// Clears all of the functions and variables from system
    /// </summary>
    public void Reset()
    {
        VisSystemMemory.Clear();
    }
}