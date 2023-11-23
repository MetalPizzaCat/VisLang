namespace VisLang;

/// <summary>
/// Gets value of the array element
/// </summary>
public class ArrayLengthNode : DataNode
{
    public ArrayLengthNode()
    {
    }

    public ArrayLengthNode(VisSystem? interpreter) : base(interpreter)
    {
    }


    public Value? GetInputArray(NodeContext? context) => Inputs.ElementAtOrDefault(0)?.GetValue(context);

    public override Value? GetValue(NodeContext? context = null)
    {
        if (GetInputArray(context)?.Data is List<Value> arr)
        {
            return new Value(new ValueTypeData(ValueType.Integer), arr.Count);
        }
        return new Value(new ValueTypeData(ValueType.Integer), 0);
    }
}