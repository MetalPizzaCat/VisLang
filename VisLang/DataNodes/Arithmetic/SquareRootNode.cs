namespace VisLang;

public class SquareRootNode  : ArithmeticOperationNodeBase
{
    public SquareRootNode (VisSystem? interpreter) : base(interpreter)
    {
    }

    public SquareRootNode () { }

    public override Value? GetValue(NodeContext? context = null)
    {
        return new Value(new ValueTypeData(ValueType.Float), Math.Sqrt(GetValueLeft(context)));
    }
}