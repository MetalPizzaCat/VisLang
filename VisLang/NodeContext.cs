namespace VisLang;

/// <summary>
/// Provides context necessary for node execution
/// </summary>
public class NodeContext
{
    public NodeContext()
    {
    }

    public NodeContext(VisSystem? interpreter, Dictionary<string, uint>? variables)
    {
        Interpreter = interpreter;
        Variables = variables;
    }

    /// <summary>
    /// What interpreter system to read and write to. If null node should use system provided via constructor
    /// </summary>
    public VisSystem? Interpreter { get; set; }

    /// <summary>
    /// Variables that node that read and write. If null variables in the system itself are used.<para></para>
    /// This can be used to create local variables, for example variables for a function
    /// </summary>
    public Dictionary<string, uint>? Variables { get; set; }

    /// <summary>
    /// The stack used to store levels of loops to be able to resume from the loop node instead of just quitting<para/>
    /// This has to be stored in the context because otherwise loops gain power to mangle execution flow
    /// </summary>
    public Stack<ExecutionNode> LoopNodeStack { get; private set; } = new();
}