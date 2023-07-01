namespace VisLang;

/// <summary>
/// Base class for all logic nodes in the VisLang
/// </summary>
public class VisNode
{
    public VisNode()
    {
    }

    public VisNode(VisSystem? interpreter)
    {
        Interpreter = interpreter;
    }

    public VisNode(Dictionary<string, uint> variableList, VisSystem? interpreter)
    {
        Interpreter = interpreter;
        _availableVariableList = variableList;
    }

    /// <summary>
    /// Additional custom debug data that can be used by the debugger for it's own purposes</para>
    /// Like storing a reference to node representation of this node in the ui
    /// </summary>
    public object? DebugData { get; set; } = null;

    /// <summary>
    /// All nodes that can be used to retrieve data.
    /// </summary>
    /// <returns></returns>
    public List<VisNode> Inputs { get; set; } = new();

    /// <summary>
    /// Reference to the actual system that stores memory and logic values
    /// </summary>
    public VisSystem? Interpreter { get; set; } = null;

    /// <summary>
    /// Local list for all available variables. <para></para>
    /// By limiting function access to a specific set of variables we can create a system for local variables
    /// </summary>
    private Dictionary<string, uint>? _availableVariableList = null;

    /// <summary>
    /// Local list for all available variables, if internal provided list is null then this will return global interpreter list<para></para>
    /// If even the interpreter is null it will return null in the end<para></para>
    /// By limiting function access to a specific set of variables we can create a system for local variables
    /// </summary>
    public Dictionary<string, uint>? AvailableVariableList => _availableVariableList ?? Interpreter?.VisSystemMemory.Variables;

    /// <summary>
    /// Get current value that this node wants to return. Each node can only have one value output
    /// <param name = "context">Additional context for the node value retrieval </param>
    /// </summary>
    public virtual Value? GetValue(NodeContext? context)
    {
        return null;
    }
}