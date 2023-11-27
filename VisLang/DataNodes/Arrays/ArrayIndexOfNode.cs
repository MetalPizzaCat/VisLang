using VisLang.Interpreter;

namespace VisLang;

/// <summary>
/// Gets value of the array element
/// </summary>
public class ArrayIndexOfNode : DataNode
{
    public ArrayIndexOfNode()
    {
    }

    public ArrayIndexOfNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public Value? GetInputArray(NodeContext? context) => Inputs.ElementAtOrDefault(0)?.GetValue(context);
    public Value? GetInputValue(NodeContext? context) => Inputs.ElementAtOrDefault(1)?.GetValue(context);

    public override Value GetValue(NodeContext? context = null)
    {
        Value? val = GetInputValue(context);
        if (val != null)
        {
            if (GetInputArray(context)?.Data is List<Value> arr)
            {
                // we have to compare data because if we don't we will be comparing the references of the storing type
                return new Value(new ValueTypeData(ValueType.Integer), (long)arr.FindIndex(item => item.EqualsTo(val)));
            }
            else if (GetInputArray(context)?.Data is string str)
            {
                if (val.TypeData.Type != ValueType.Char)
                {
                    throw new ValueTypeMismatchException($"Expected a value of char got {val.TypeData.Type}", this);
                }
                if (val.Data == null)
                {
                    throw new VisLangNullException("Searched value in IndexOf must not be null when searching in string", this);
                }
                return new Value(new ValueTypeData(ValueType.Integer), (long)str.IndexOf((char)val.Data));
            }
        }
        return new Value(new ValueTypeData(ValueType.Integer), -1);
    }
}