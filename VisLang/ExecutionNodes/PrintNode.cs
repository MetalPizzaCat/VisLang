namespace VisLang;

public class PrintNode : ExecutionNode
{
    public PrintNode() { }
    public PrintNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public override void Execute(NodeContext? context = null)
    {
        base.Execute(context);
        DataNode? srs = Inputs.FirstOrDefault();
        if (srs == null)
        {
            Interpreter?.AddOutput("Input from print was missing");
            return;
        }
        if (srs.GetValue(context) == null)
        {
            Interpreter?.AddOutput($"Printed value is null");
            return;
        }
        Interpreter?.AddOutput(srs.GetValue(context)?.TryAsString() ?? "Attempted to print value but value was null so instead you get this string :3");
    }
}