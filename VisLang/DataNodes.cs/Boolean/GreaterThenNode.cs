namespace VisLang;

public class GreaterThenNode : BinaryOperationNodeBase
{
    public GreaterThenNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public override Value? GetValue(NodeContext? context = null)
    {
        if (GetValueLeft(context).TypeData.Type != GetValueRight(context).TypeData.Type)
        {
            throw new Interpreter.ValueTypeMismatchException($"Attempted to compare values but types do no match. Value A is {GetValueLeft(context).TypeData.Type} and value B is {GetValueRight(context).TypeData.Type}", this);
        }
        if (GetValueLeft(context).TypeData.Type == ValueType.Integer)
        {
            return new Value(new ValueTypeData(ValueType.Bool), GetValueLeft(context).AsInteger() > GetValueRight(context).AsInteger());
        }
        if (GetValueLeft(context).TypeData.Type == ValueType.Float)
        {
            return new Value(new ValueTypeData(ValueType.Bool), GetValueLeft(context).AsFloat() > GetValueRight(context).AsFloat());
        }
        throw new Interpreter.ValueTypeMismatchException($"Numeric comparisons can only be performed on Integers and Floats,type is {GetValueLeft(context).TypeData.Type}", this);
    }
}