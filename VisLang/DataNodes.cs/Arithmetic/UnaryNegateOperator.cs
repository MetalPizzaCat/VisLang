namespace VisLang;

public class UnaryNegateOperationNodeBase : DataNode
{
    public float DefaultValue { get; set; } = 0f;

    public float Value => Inputs.FirstOrDefault()?.GetValue()?.AsFloat() ?? DefaultValue;

    public override Value? GetValue()
    {
        return new Value(ValueType.Float, -Value);
    }
}