namespace VisLang;

/// <summary>
/// Returns value of the variable stored in the system memory
/// </summary>
public class ValueGetAddressNode : DataNode
{
    public ValueGetAddressNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public override Value? GetValue(NodeContext? context = null)
    {
        if (Inputs.FirstOrDefault() == null)
        {
            return null;
        }
        if (Inputs.FirstOrDefault()?.GetValue(context) == null)
        {
            return null;
        }
        if (Inputs.FirstOrDefault()?.GetValue(context).ValueType != ValueType.Address)
        {
            return null;
        }
        return new Value(ValueType.Address, (uint)(Inputs.FirstOrDefault()?.GetValue(context)?.Address ?? 0));
    }
}