namespace VisLang;

public class PrintNode : ExecutionNode
{
    
    public PrintNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public override void Execute()
    {
        base.Execute();
        Interpreter?.AddOutput(Inputs.FirstOrDefault()?.GetValue()?.AsString() ?? "Hello, blessed sigmar, the world drowns in memory leaks!");
    }
}