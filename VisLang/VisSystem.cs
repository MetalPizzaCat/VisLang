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

    public VisSystemMemory VisSystemMemory { get; set; } = new();

    /// <summary>
    /// All of the procedures loaded in the memory that can be referenced by nodes inside this system
    /// </summary>
    public List<VisProcedure> Procedures { get; set; } = new();

    public VisProcedure? GetProcedure(string name) => Procedures.Find(p => p.Name == name);

    public List<VisFunction> Functions { get; set; } = new();

    public VisFunction? GetFunction(string name) => Functions.Find(p => p.Name == name);

    /// <summary>
    /// The stack used to store levels of loops to be able to resume from the loop node instead of just quitting
    /// </summary>
    public Stack<ExecutionNode> LoopNodeStack { get; private set; } = new();

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
        Stack<ExecutionNode>? stack = context?.LoopNodeStack ?? LoopNodeStack;
        ExecutionNode? temp = null;
        stack.TryPeek(out temp);
        // the check is there to prevent creating more loops by returning to the head of the loop
        if (Current is ForNodeBase && Current != temp)
        {
            stack.Push(Current);
        }
        Current.Execute(context);
        if (Current is ForNodeBase loop && loop.WasFinished)
        {
            stack.Pop();
        }
        // some nodes, like callable, change current node themselves during execution
        // because of this we should only check if there is none
        Current = Current?.GetNext();
        if (Current == null && stack.Count > 0)
        {
            Current = stack.Pop();
        }
        return Current != null;
    }

    /// <summary>
    /// Begins the execution from the first node and continues as long as there are nodes in the list
    /// </summary>
    public void Execute()
    {
        Entrance?.Execute();
        // after we execute we should check if it's a loop because otherwise programs starting with a loop
        // will not work as well
        if (Entrance is ForNodeBase)
        {
            LoopNodeStack.Push(Entrance);
        }
        Current = Entrance?.GetNext();
        // we do we execute both in condition AND body?
        // i've written this long time ago and i do not remember why it is like this
        while (ExecuteNext(null) && Current != null)
        {
            ExecutionNode? temp = null;
            LoopNodeStack.TryPeek(out temp);
            if (Current is ForNodeBase && Current != temp)
            {
                LoopNodeStack.Push(Current);
            }
            Current.Execute();
            if (Current is ForNodeBase loop && loop.WasFinished)
            {
                LoopNodeStack.Pop();
            }
            Current = Current?.GetNext();
            // we might reach end of the execution but we might be in a loop
            // so we try to pop 
            if (Current == null && LoopNodeStack.Count > 0)
            {
                Current = LoopNodeStack.Pop();
            }
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