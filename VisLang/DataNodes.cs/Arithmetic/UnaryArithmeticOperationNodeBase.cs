namespace VisLang;

public class UnaryArithmeticOperationNodeBase : DataNode
{
    public float DefaultValue {get; set; } = 0f;

    public float GetInputValue(NodeContext? context) => Inputs.FirstOrDefault()?.GetValue(context)?.AsFloat() ?? DefaultValue;

    public UnaryArithmeticOperationNodeBase(VisSystem? interpreter) : base(interpreter)
    {
    }

    public UnaryArithmeticOperationNodeBase()
    {
    }
}