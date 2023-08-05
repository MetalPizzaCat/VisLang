namespace VisLang;

public class UnaryNegateOperationNodeBase : DataNode
{
    public double DefaultValue { get; set; } = 0f;

    public double GetInputValue(NodeContext? context) => Inputs.FirstOrDefault()?.GetValue(context)?.AsFloat() ?? DefaultValue;

    public override Value? GetValue(NodeContext? context = null)
    {
        return new Value(new ValueTypeData(ValueType.Float), -GetInputValue(context));
    }
}