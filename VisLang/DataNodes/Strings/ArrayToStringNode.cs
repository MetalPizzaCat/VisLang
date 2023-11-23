using VisLang.Interpreter;

namespace VisLang;

/// <summary>
/// Converts an string to an array of chars for further operations
/// </summary>
public class ArrayToStringNode : DataNode
{
    public ArrayToStringNode()
    {
    }

    public ArrayToStringNode(VisSystem? interpreter) : base(interpreter)
    {
    }


    public Value? GetInputArray(NodeContext? context) => Inputs.ElementAtOrDefault(0)?.GetValue(context);

    public override Value? GetValue(NodeContext? context = null)
    {
        if (GetInputArray(context)?.Data is List<Value> arr)
        {
            return new Value
            (
                new ValueTypeData(ValueType.String),
                string.Join(null, arr.Select(p => p.Data))
            );
        }
        else
        {
            throw new ValueTypeMismatchException($"Can't convert array to string because argument is not an array. Expected an array got {GetInputArray(context)?.GetTypeString()}", this);
        }
    }
}