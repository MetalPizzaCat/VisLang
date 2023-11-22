namespace VisLang;

/// <summary>
/// This node iterates over the contents of the array returning the current element <para/>
/// it has: Exec input, array input
/// </summary>
public class ForeachNode : ExecutionNode
{
    public ForeachNode()
    {
    }

    public ForeachNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public ExecutionNode? FailureNext { get; set; } = null;

    public override ExecutionNode? GetNext() => (Inputs.FirstOrDefault()?.GetValue(null)?.TryAsBool() ?? false) ? DefaultNext : FailureNext;
}