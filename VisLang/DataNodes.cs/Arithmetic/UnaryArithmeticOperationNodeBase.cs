namespace VisLang;

public class UnaryArithmeticOperationNodeBase : DataNode
{
    public float DefaultValue {get; set; } = 0f;

    public float Value => Inputs.FirstOrDefault()?.GetValue()?.AsFloat() ?? DefaultValue;

    public UnaryArithmeticOperationNodeBase(VisSystem? interpreter) : base(interpreter)
    {
    }

    public UnaryArithmeticOperationNodeBase()
    {
    }
}