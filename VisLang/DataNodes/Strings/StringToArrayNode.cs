using VisLang.Interpreter;

namespace VisLang;

/// <summary>
/// Converts an string to an array of chars for further operations
/// </summary>
public class StringToArrayNode : DataNode
{
    public StringToArrayNode()
    {
    }

    public StringToArrayNode(VisSystem? interpreter) : base(interpreter)
    {
    }


    public Value? GetInputString(NodeContext? context) => Inputs.ElementAtOrDefault(0)?.GetValue(context);

    public override Value? GetValue(NodeContext? context = null)
    {
        if (GetInputString(context)?.Data is string str)
        {
            return new Value
            (
                new ValueTypeData(ValueType.Array, ValueType.Char),
                str.Select(ch => new Value(new ValueTypeData(ValueType.Char), ch)).ToList()
            );
        }
        else
        {
            throw new ValueTypeMismatchException($"Can't convert string to array because argument is not string. Expected a string got {GetInputString(context)?.GetTypeString()}", this);
        }
    }
}