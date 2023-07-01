namespace VisLang;

public class DivisionNode : ArithmeticOperationNodeBase
{
    public DivisionNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public DivisionNode() { }

    public override Value? GetValue()
    {
        return new Value(ValueType.Float, ValueLeft / ValueRight);
    }
}