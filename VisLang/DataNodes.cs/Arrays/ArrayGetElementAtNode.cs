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

    public Value? Array => Inputs.ElementAtOrDefault(0)?.GetValue();
    public int Index => Inputs.ElementAtOrDefault(1)?.GetValue()?.AsInteger() ?? DefaultIndex;

    public override Value? GetValue()
    {
        if (Array?.Data is List<Value> arr)
        {
            return arr[Index];
        }
        return null;
    }
}