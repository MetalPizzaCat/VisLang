namespace VisLang;

/// <summary>
/// Node that sets value of the the array element. Array is first input, index is second and value is third
/// </summary>
public class ArraySetElementAtNode : ExecutionNode
{
    public int DefaultIndex = 0;
    public Value? Array => Inputs.ElementAtOrDefault(0)?.GetValue();
    public int Index => Inputs.ElementAtOrDefault(1)?.GetValue()?.AsInteger() ?? DefaultIndex;
    public object? Value => Inputs.ElementAtOrDefault(2)?.GetValue()?.Data;

    public ValueType? ValueToSetType => Inputs.ElementAtOrDefault(2)?.GetValue()?.ValueType;

    public ArraySetElementAtNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public ArraySetElementAtNode()
    {
    }
    public override void Execute()
    {
        if (Interpreter == null)
        {
            throw new NullReferenceException("Interpreter system is null");
        }
        if (Array == null)
        {
            throw new NullReferenceException("Attempted set value of the array element but array is null");
        }
        if (Value == null)
        {
            throw new NullReferenceException("Attempted set value of the array element but provided value is null");
        }
        if (Array.Data is List<Value> arr)
        {
            arr[Index].Data = Value;
        }
    }
}