namespace VisLang;

public class PrintNode : ExecutionNode
{
    public PrintNode() { }
    public PrintNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public override void Execute()
    {
        base.Execute();
        Interpreter?.AddOutput(Inputs.FirstOrDefault()?.GetValue()?.TryAsString() ?? "Attempted to print value but value was null so instead you get this string :3");
    }
}