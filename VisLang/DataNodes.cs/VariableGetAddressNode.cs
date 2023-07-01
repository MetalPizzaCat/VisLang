namespace VisLang;

/// <summary>
/// Returns address of the variable stored in the system memory
/// </summary>
public class VariableGetAddressNode : DataNode
{
    public VariableGetAddressNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public string Name { get; set; } = "Default";
    public override Value? GetValue(NodeContext? context = null)
    {
        if (Interpreter?.VisSystemMemory.GetValue(Name, null) == null)
        {
            return new Value(ValueType.Address, 0);
        }
        return new Value(ValueType.Address, Interpreter?.VisSystemMemory.GetValue(Name, null).Address);
    }
}