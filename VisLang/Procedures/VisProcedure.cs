namespace VisLang;

/// <summary>
/// Storage class for storing information about callable procedure
/// </summary>
public class VisProcedure : VisCallable
{
    public VisProcedure() { }

    /// <summary>
    /// The first node that stores rest of the procedure execute tree
    /// </summary>
    public ExecutionNode? ProcedureNodesRoot { get; set; }
}