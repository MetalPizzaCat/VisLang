namespace VisLang;

/// <summary>
/// Node that sets value of the the array element. Array is first input, index is second and value is third
/// </summary>
public class ArrayAppendElement : ExecutionNode
{
    public Value? Array => Inputs.ElementAtOrDefault(0)?.GetValue();
    public object? Value => Inputs.ElementAtOrDefault(1)?.GetValue()?.Data;

    public ValueType? ValueToSetType => Inputs.ElementAtOrDefault(1)?.GetValue()?.ValueType;

    public ArrayAppendElement(VisSystem? interpreter) : base(interpreter)
    {
    }

    public ArrayAppendElement()
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
        if (Array.ValueType != ValueType.Array)
        {
            throw new Interpreter.ValueTypeMismatchException($"Attempted to append to array but given value is not an array. Value type: {Array.ValueType}", this);
        }
        if (Array.ArrayDataType != null && ValueToSetType != null)
        {
            if (Array.ArrayDataType.Value != ValueToSetType.Value)
            {
                throw new Interpreter.ValueTypeMismatchException($"Attempted to add element to the array but type does not match array data. Expected {Array.ArrayDataType.Value}, got {ValueToSetType.Value}", this);
            }
        }

        if (Array.Data is List<Value> arr && ValueToSetType != null)
        {
            arr.Add(new Value(ValueToSetType.Value, Value));
        }
    }
}