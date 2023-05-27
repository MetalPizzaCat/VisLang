namespace VisLang;

/// <summary>
/// Special node that simply returns a constant value whenever you ask it
/// This is not meant to be a special node, but it exists to allow for more dynamic node creation
/// </summary>
public class VariableGetConstNode : DataNode
{
    public VariableGetConstNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public string Name { get; set; } = "Default";
    public Value Value { get; set; } = new Value(ValueType.Number, false, 0);
    public override Value? GetValue()
    {
        return Value;
    }
}