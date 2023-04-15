namespace VisLang;

public class AdditionNode : ArithmeticOperationNodeBase
{
    public AdditionNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public override Value? GetValue()
    {
        return new Value(VariableType.Number, false, ValueLeft + ValueRight);
    }
}