namespace VisLang;

public class AdditionNode : ArithmeticOperationNodeBase
{
    public AdditionNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public AdditionNode() { }

    public override Value? GetValue()
    {
        return new Value(ValueType.Float, ValueLeft + ValueRight);
    }
}