namespace VisLang;

public class AdditionNode : ArithmeticOperationNodeBase
{
    public AdditionNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public AdditionNode() { }

    public override Value? GetValue(NodeContext? context = null)
    {
        return new Value(ValueType.Float, GetValueLeft(context) + GetValueRight(context));
    }
}