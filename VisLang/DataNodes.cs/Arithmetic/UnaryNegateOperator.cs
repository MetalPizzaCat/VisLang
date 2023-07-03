namespace VisLang;

public class UnaryNegateOperationNodeBase : DataNode
{
    public float DefaultValue { get; set; } = 0f;

    public float GetInputValue(NodeContext? context) => Inputs.FirstOrDefault()?.GetValue(context)?.AsFloat() ?? DefaultValue;

    public override Value? GetValue(NodeContext? context = null)
    {
        return new Value(ValueType.Float, -GetInputValue(context));
    }
}