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

    public int DefaultIndex = 0;

    public Value? GetInputArray(NodeContext? context) => Inputs.ElementAtOrDefault(0)?.GetValue(context);
    public int GetInputIndex(NodeContext? context) => Inputs.ElementAtOrDefault(1)?.GetValue(context)?.AsInteger() ?? DefaultIndex;

    public override Value? GetValue(NodeContext? context = null)
    {
        if (GetInputArray(context)?.Data is List<Value> arr)
        {
            return arr[GetInputIndex(context)];
        }
        return null;
    }
}