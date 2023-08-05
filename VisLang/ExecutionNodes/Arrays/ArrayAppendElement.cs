namespace VisLang;

/// <summary>
/// Node that sets value of the the array element. Array is first input, index is second and value is third
/// </summary>
public class ArrayAppendElement : ExecutionNode
{
    public Value? GetInputArray(NodeContext? context) => Inputs.ElementAtOrDefault(0)?.GetValue(context);
    public object? GetInputValue(NodeContext? context) => Inputs.ElementAtOrDefault(1)?.GetValue(context)?.Data;

    public ValueTypeData? GetValueToSetType(NodeContext? context) => Inputs.ElementAtOrDefault(1)?.GetValue(context)?.TypeData;

    public ArrayAppendElement(VisSystem? interpreter) : base(interpreter)
    {
    }

    public ArrayAppendElement()
    {
    }
    public override void Execute(NodeContext? context = null)
    {
        if (Interpreter == null)
        {
            throw new NullReferenceException("Interpreter system is null");
        }
        Value? arr = GetInputArray(context);
        object? value = GetInputValue(context);
        ValueTypeData? type = GetValueToSetType(context);
        if (arr == null)
        {
            throw new NullReferenceException("Attempted set value of the array element but array is null");
        }
        if (value == null)
        {
            throw new NullReferenceException("Attempted set value of the array element but provided value is null");
        }
        if (arr.ValueType != ValueType.Array)
        {
            throw new Interpreter.ValueTypeMismatchException($"Attempted to append to array but given value is not an array. Value type: {arr.ValueType}", this);
        }
        if (arr.ArrayDataType != null && type != null)
        {
            if (arr.ArrayDataType.Value != type.Value.Type)
            {
                throw new Interpreter.ValueTypeMismatchException($"Attempted to add element to the array but type does not match array data. Expected {arr.ArrayDataType.Value}, got {type.Value.Type}", this);
            }
        }

        if (arr.Data is List<Value> internalArray && type != null)
        {
            //TODO: account for appending arrays
            internalArray.Add(new Value(type.Value, GetInputValue(context)));
        }
    }
}