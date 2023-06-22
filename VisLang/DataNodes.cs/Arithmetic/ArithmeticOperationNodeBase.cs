namespace VisLang;

public class ArithmeticOperationNodeBase : DataNode
{
    public float DefaultValueLeft { get; set; } = 0f;
    public float DefaultValueRight { get; set; } = 0f;

    public float ValueLeft => Inputs.FirstOrDefault()?.GetValue()?.AsFloat() ?? DefaultValueLeft;

    public float ValueRight => Inputs.ElementAtOrDefault(1)?.GetValue()?.AsFloat() ?? DefaultValueRight;

    public ArithmeticOperationNodeBase(VisSystem? interpreter) : base(interpreter)
    {
    }

    public ArithmeticOperationNodeBase()
    {
    }
}