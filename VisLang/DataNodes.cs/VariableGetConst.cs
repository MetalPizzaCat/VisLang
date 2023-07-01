namespace VisLang;

/// <summary>
/// Special node that simply returns a constant value whenever you ask it
/// This is not meant to be a special node, but it exists to allow for more dynamic node creation
/// </summary>
public class VariableGetConstNode : DataNode
{

    /// <summary>
    /// Creates new const node. There is no need for this object to have reference to interpreter system because 
    /// it should *never* access it as value is written at preprocessing stage
    /// </summary>
    public VariableGetConstNode() { }

    public string Name { get; set; } = "Default";
    public Value Value { get; set; } = new Value(ValueType.Float, 0);
    public override Value? GetValue(NodeContext? context = null)
    {
        return Value;
    }
}