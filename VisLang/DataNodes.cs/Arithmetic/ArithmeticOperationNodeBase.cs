namespace VisLang;

public class ArithmeticOperationNodeBase : DataNode
{
    public float DefaultValueLeft { get; set; } = 0f;
    public float DefaultValueRight { get; set; } = 0f;

    public float ValueLeft => Inputs.FirstOrDefault()?.GetValue()?.AsNumber() ?? DefaultValueLeft;

    public float ValueRight => Inputs.ElementAtOrDefault(1)?.GetValue()?.AsNumber() ?? DefaultValueRight;

    public ArithmeticOperationNodeBase(VisSystem? interpreter) : base(interpreter)
    {
    }
}