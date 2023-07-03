namespace VisLang;

public class BinaryOperationNodeBase : DataNode
{
    public BinaryOperationNodeBase(VisSystem? interpreter) : base(interpreter)
    {
    }

    public Value DefaultValueLeft { get; set; } = new Value(ValueType.Bool, false);
    public Value DefaultValueRight { get; set; } = new Value(ValueType.Bool, false);

    public Value GetValueLeft(NodeContext? context) => Inputs.FirstOrDefault()?.GetValue(context) ?? DefaultValueLeft;

    public Value GetValueRight(NodeContext? context) => Inputs.ElementAtOrDefault(1)?.GetValue(context) ?? DefaultValueRight;
}