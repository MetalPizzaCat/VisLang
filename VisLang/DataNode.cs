namespace VisLang;

/// <summary>
/// Data node is a type of node that can not be executed by can be used to generate or get data<para/>
/// For example DataNode can be used to implement Variable value getting node or arithmetic operations
/// </summary>
public class DataNode : VisNode
{
    public DataNode()
    {
    }

    public DataNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    /// <summary>
    /// Get current value that this node wants to return. Each node can only have one value output
    /// <param name = "context">Additional context for the node value retrieval </param>
    /// </summary>
    public virtual Value? GetValue(NodeContext? context)
    {
        return null;
    }
}
