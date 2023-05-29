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

    public virtual Value? GetValue()
    {
        return null;
    }
}