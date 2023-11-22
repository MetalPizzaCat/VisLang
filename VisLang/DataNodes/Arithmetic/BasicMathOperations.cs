namespace VisLang;

public class ArithmeticOperationNodeBase : DataNode
{
    public double DefaultValueLeft { get; set; } = 0f;
    public double DefaultValueRight { get; set; } = 0f;

    public double GetValueLeft(NodeContext? context) => Inputs.FirstOrDefault()?.GetValue(context)?.AsFloat() ?? DefaultValueLeft;

    public double GetValueRight(NodeContext? context) => Inputs.ElementAtOrDefault(1)?.GetValue(context)?.AsFloat() ?? DefaultValueRight;

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
        return new Value(new ValueTypeData(ValueType.Float), GetValueLeft(context) / GetValueRight(context));
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
        return new Value(new ValueTypeData(ValueType.Float), GetValueLeft(context) * GetValueRight(context));
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
        return new Value(new ValueTypeData(ValueType.Float), GetValueLeft(context) - GetValueRight(context));
    }
}