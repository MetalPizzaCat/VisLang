namespace VisLang;

public class BinaryOperationNodeBase : DataNode
{
    public BinaryOperationNodeBase(VisSystem? interpreter) : base(interpreter)
    {
    }

    public Value DefaultValueLeft { get; set; } = new Value(ValueType.Bool, false, false);
    public Value DefaultValueRight { get; set; } = new Value(ValueType.Bool, false, false);

    public Value ValueLeft => Inputs.FirstOrDefault()?.GetValue() ?? DefaultValueLeft;

    public Value ValueRight => Inputs.ElementAtOrDefault(1)?.GetValue() ?? DefaultValueRight;
}