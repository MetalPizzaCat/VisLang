namespace VisLang;

/// <summary>
/// Storage class for storing information about callable procedure
/// </summary>
public class VisProcedure
{
    public string Name { get; set; } = "ProcedurePlaceHolderNameHereHelloGoodbye";

    public Dictionary<string, ValueType> Arguments { get; set; } = new();

    /// <summary>
    /// The first node that stores rest of the procedure execute tree
    /// </summary>
    public ExecutionNode? ProcedureNodesRoot { get; set; }

    /// <summary>
    /// All of the nodes present in the system
    /// </summary>
    public List<VisNode> Nodes { get; set; } = new();

    /// <summary>
    /// Type of the output value or null if procedure returns no value
    /// </summary>
    public ValueType? OutputValueType { get; set; }
}