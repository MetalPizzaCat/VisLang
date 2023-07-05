namespace VisLang;

public class AdditionNode : ArithmeticOperationNodeBase
{
    public AdditionNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public AdditionNode() { }

    public override Value? GetValue(NodeContext? context = null)
    {
        return new Value(new ValueTypeData(ValueType.Float), GetValueLeft(context) + GetValueRight(context));
    }
}