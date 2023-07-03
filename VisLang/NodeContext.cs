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
}