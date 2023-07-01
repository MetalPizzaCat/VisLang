namespace VisLang;

public class PrintNode : ExecutionNode
{
    public PrintNode() { }
    public PrintNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public override void Execute(NodeContext? context = null)
    {
        base.Execute();
        Interpreter?.AddOutput(Inputs.FirstOrDefault()?.GetValue(context)?.TryAsString() ?? "Attempted to print value but value was null so instead you get this string :3");
    }
}