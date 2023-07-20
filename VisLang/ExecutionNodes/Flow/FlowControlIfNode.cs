namespace VisLang;

/// <summary>
/// Node that can select which branch of execution to use based on value of the input<para></para>
/// For convenience it uses DefaultNode for condition being true and FailureNext for condition being falls
/// </summary>
public class FlowControlIfNode : ExecutionNode
{
    public FlowControlIfNode()
    {
    }

    public FlowControlIfNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public ExecutionNode? FailureNext { get; set; } = null;

    public override ExecutionNode? GetNext() => (Inputs.FirstOrDefault()?.GetValue(null)?.TryAsBool() ?? false) ? DefaultNext : FailureNext;
}