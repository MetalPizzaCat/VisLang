namespace VisLang;

/// <summary>
/// Gets value of the String element
/// </summary>
public class StringGetElementAtNode : DataNode
{
    public StringGetElementAtNode()
    {
    }

    public StringGetElementAtNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public long DefaultIndex = 0;

    public Value? GetInputString(NodeContext? context) => Inputs.ElementAtOrDefault(0)?.GetValue(context);
    public long GetInputIndex(NodeContext? context) => Inputs.ElementAtOrDefault(1)?.GetValue(context)?.AsInteger() ?? DefaultIndex;

    public override Value? GetValue(NodeContext? context = null)
    {
        if (GetInputString(context)?.Data is string arr)
        {
            return new Value(new ValueTypeData(ValueType.Char), arr[(int)GetInputIndex(context)]);
        }
        return null;
    }
}