namespace VisLang;

public class SubtractionNode : ArithmeticOperationNodeBase
{
    public SubtractionNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public SubtractionNode() { }

    public override Value? GetValue()
    {
        return new Value(ValueType.Float, ValueLeft - ValueRight);
    }
}