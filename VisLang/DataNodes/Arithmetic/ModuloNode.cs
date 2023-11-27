namespace VisLang;

public class ModuloNode : ArithmeticOperationNodeBase
{
    public ModuloNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public ModuloNode() { }

    public override Value? GetValue(NodeContext? context = null)
    {
        return new Value(new ValueTypeData(ValueType.Integer), GetValueLeft(context) % GetValueRight(context));
    }
}