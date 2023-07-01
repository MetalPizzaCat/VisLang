namespace VisLang;

public class MultiplicationNode : ArithmeticOperationNodeBase
{
    public MultiplicationNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public MultiplicationNode() { }

    public override Value? GetValue()
    {
        return new Value(ValueType.Float, ValueLeft * ValueRight);
    }
}