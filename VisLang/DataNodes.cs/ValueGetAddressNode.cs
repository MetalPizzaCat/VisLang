namespace VisLang;

/// <summary>
/// Returns value of the variable stored in the system memory
/// </summary>
public class ValueGetAddressNode : DataNode
{
    public ValueGetAddressNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public override Value? GetValue()
    {
        if (Inputs.FirstOrDefault() == null)
        {
            return null;
        }
        if (Inputs.FirstOrDefault().GetValue() == null)
        {
            return null;
        }
        if (Inputs.FirstOrDefault().GetValue().ValueType != ValueType.Address)
        {
            return null;
        }
        return new Value(ValueType.Address, (uint)(Inputs.FirstOrDefault().GetValue().Address ?? 0));
    }
}