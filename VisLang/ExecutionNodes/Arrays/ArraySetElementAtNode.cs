namespace VisLang;

/// <summary>
/// Node that sets value of the the array element. Array is first input, index is second and value is third
/// </summary>
public class ArraySetElementAtNode : ExecutionNode
{
    public long DefaultIndex = 0;
    public Value? GetInputArray(NodeContext? context) => Inputs.ElementAtOrDefault(0)?.GetValue(context);
    public long GetInputIndex(NodeContext? context) => Inputs.ElementAtOrDefault(1)?.GetValue(context)?.AsInteger() ?? DefaultIndex;
    public object? GetInputValue(NodeContext? context) => Inputs.ElementAtOrDefault(2)?.GetValue(context)?.Data;

    public ValueType? GetValueToSetType(NodeContext? context) => Inputs.ElementAtOrDefault(2)?.GetValue(context)?.ValueType;

    public ArraySetElementAtNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public ArraySetElementAtNode()
    {
    }
    public override void Execute(NodeContext? context = null)
    {
        if (Interpreter == null)
        {
            throw new NullReferenceException("Interpreter system is null");
        }
        if (GetInputArray(context) == null)
        {
            throw new NullReferenceException("Attempted set value of the array element but array is null");
        }
        if (GetInputValue(context) == null)
        {
            throw new NullReferenceException("Attempted set value of the array element but provided value is null");
        }
        if (GetInputArray(context)?.Data is List<Value> arr)
        {
            arr[(int)GetInputIndex(context)] = Inputs.ElementAtOrDefault(2)?.GetValue(context) ?? throw new NullReferenceException("Value must not be null");
        }
    }
}