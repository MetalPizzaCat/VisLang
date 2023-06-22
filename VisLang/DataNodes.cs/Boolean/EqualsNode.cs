namespace VisLang;

public class EqualsNode : BinaryOperationNodeBase
{
    public EqualsNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public override Value? GetValue()
    {
        return new Value(ValueType.Bool, ValueLeft.EqualsTo(ValueRight));
    }
}