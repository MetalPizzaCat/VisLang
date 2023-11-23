namespace VisLang;

public class AdditionIntNode : DataNode
{
    public long DefaultValueLeft { get; set; } = 0;
    public long DefaultValueRight { get; set; } = 0;

    public long GetValueLeft(NodeContext? context) => Inputs.FirstOrDefault()?.GetValue(context)?.AsInteger() ?? DefaultValueLeft;

    public long GetValueRight(NodeContext? context) => Inputs.ElementAtOrDefault(1)?.GetValue(context)?.AsInteger() ?? DefaultValueRight;

    public AdditionIntNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public AdditionIntNode() { }

    public override Value? GetValue(NodeContext? context = null)
    {
        return new Value(new ValueTypeData(ValueType.Integer), GetValueLeft(context) + GetValueRight(context));
    }
}