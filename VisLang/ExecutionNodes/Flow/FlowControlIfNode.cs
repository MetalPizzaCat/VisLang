namespace VisLang;

public class FlowControlIfNode : ExecutionNode
{
    public FlowControlIfNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public ExecutionNode? SuccessNext { get; set; } = null;

    public ExecutionNode? FailureNext { get; set; } = null;

    public override ExecutionNode? GetNext() => (Inputs.FirstOrDefault()?.GetValue()?.TryAsBool() ?? false) ? SuccessNext : FailureNext;
}