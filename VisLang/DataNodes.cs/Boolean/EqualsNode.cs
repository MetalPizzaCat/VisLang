namespace VisLang;

public class EqualsNode : BinaryOperationNodeBase
{
    public EqualsNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public override Value? GetValue(NodeContext? context = null)
    {
        return new Value(new ValueTypeData(ValueType.Bool), GetValueLeft(context).EqualsTo(GetValueRight(context)));
    }
}