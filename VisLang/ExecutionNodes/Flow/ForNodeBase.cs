namespace VisLang;

/// <summary>
/// Base node for all "for" based loops
/// </summary>
public class ForNodeBase : ExecutionNode
{
    public ForNodeBase()
    {
    }

    public ForNodeBase(VisSystem? interpreter) : base(interpreter)
    {
    }

    /// <summary>
    /// Name of the variable used to store iteration value
    /// </summary>
    public string IteratorVariableName { get; set; } = string.Empty;

    /// <summary>
    /// If set to true loop node should exit no matter if the iteration was finished or not
    /// </summary>
    public bool WasFinished { get; protected set; }

    public virtual void FinishLoop(NodeContext? context = null)
    {
        WasFinished = true;
        Interpreter?.VisSystemMemory.FreeVariable(IteratorVariableName, context?.Variables);
    }

    /// <summary>
    /// Node that will be executed AFTER execution finished(normally or via break)
    /// </summary>
    public ExecutionNode? FinishedNext { get; set; } = null;

    public override ExecutionNode? GetNext() => WasFinished ? FinishedNext : DefaultNext;
}