namespace VisLang;

public class UnaryArithmeticOperationNodeBase : DataNode
{
    public double DefaultValue {get; set; } = 0f;

    public double GetInputValue(NodeContext? context) => Inputs.FirstOrDefault()?.GetValue(context)?.AsFloat() ?? DefaultValue;

    public UnaryArithmeticOperationNodeBase(VisSystem? interpreter) : base(interpreter)
    {
    }

    public UnaryArithmeticOperationNodeBase()
    {
    }
}