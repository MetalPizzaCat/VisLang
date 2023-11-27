namespace VisLang;

/// <summary>
/// Gets value of the array element
/// </summary>
public class ArrayGetElementAtNode : DataNode
{
    public ArrayGetElementAtNode()
    {
    }

    public ArrayGetElementAtNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public long DefaultIndex = 0;

    public Value? GetInputArray(NodeContext? context) => Inputs.ElementAtOrDefault(0)?.GetValue(context);
    public long GetInputIndex(NodeContext? context) => Inputs.ElementAtOrDefault(1)?.GetValue(context)?.AsInteger() ?? DefaultIndex;

    public override Value? GetValue(NodeContext? context = null)
    {
        if (GetInputArray(context)?.Data is List<Value> arr)
        {
            return arr[(int)GetInputIndex(context)];
        }
        else if (GetInputArray(context)?.Data is string str)
        {
            return new Value(new ValueTypeData(ValueType.Char), str[(int)GetInputIndex(context)]);
        }
        return null;
    }
}