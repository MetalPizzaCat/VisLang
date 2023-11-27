namespace VisLang;

public class EqualsNode : BinaryOperationNodeBase
{
    public EqualsNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public EqualsNode()
    {
    }

    public override Value? GetValue(NodeContext? context = null)
    {
        if (GetValueLeft(context).TypeData.Type != GetValueRight(context).TypeData.Type)
        {
            throw new Interpreter.ValueTypeMismatchException($"Attempted to compare values but types do no match. Value A is {GetValueLeft(context).TypeData.Type} and value B is {GetValueRight(context).TypeData.Type}", this);
        }
        return new Value(new ValueTypeData(ValueType.Bool), GetValueLeft(context).EqualsTo(GetValueRight(context)));
    }
}