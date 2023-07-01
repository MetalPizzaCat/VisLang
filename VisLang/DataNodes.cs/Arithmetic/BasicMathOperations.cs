namespace VisLang;

public class ArithmeticOperationNodeBase : DataNode
{
    public float DefaultValueLeft { get; set; } = 0f;
    public float DefaultValueRight { get; set; } = 0f;

    public float GetValueLeft(NodeContext? context) => Inputs.FirstOrDefault()?.GetValue(context)?.AsFloat() ?? DefaultValueLeft;

    public float GetValueRight(NodeContext? context) => Inputs.ElementAtOrDefault(1)?.GetValue(context)?.AsFloat() ?? DefaultValueRight;

    public ArithmeticOperationNodeBase(VisSystem? interpreter) : base(interpreter)
    {
    }

    public ArithmeticOperationNodeBase()
    {
    }
}

public class DivisionNode : ArithmeticOperationNodeBase
{
    public DivisionNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public DivisionNode() { }

    public override Value? GetValue(NodeContext? context = null)
    {
        return new Value(ValueType.Float, GetValueLeft(context) / GetValueRight(context));
    }
}

public class MultiplicationNode : ArithmeticOperationNodeBase
{
    public MultiplicationNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public MultiplicationNode() { }

    public override Value? GetValue(NodeContext? context = null)
    {
        return new Value(ValueType.Float, GetValueLeft(context) * GetValueRight(context));
    }
}

public class SubtractionNode : ArithmeticOperationNodeBase
{
    public SubtractionNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public SubtractionNode() { }

    public override Value? GetValue(NodeContext? context = null)
    {
        return new Value(ValueType.Float, GetValueLeft(context) - GetValueRight(context));
    }
}