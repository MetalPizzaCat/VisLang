namespace VisLang;

/// <summary>
/// This node iterates over the contents of the array returning the current element <para/>
/// it has: Exec input, array input
/// </summary>
public class ForEachNode : ForNodeBase
{
    public ForEachNode()
    {
    }

    public ForEachNode(VisSystem? interpreter) : base(interpreter)
    {
    }



    public override ExecutionNode? GetNext()
    {
        throw new NotImplementedException("This function doesn't yet implement proper execution");
    }
///=> (Inputs.FirstOrDefault()?.GetValue(null)?.TryAsBool() ?? false) ? DefaultNext : FailureNext;
}