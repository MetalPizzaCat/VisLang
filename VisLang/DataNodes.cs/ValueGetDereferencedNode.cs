namespace VisLang;

/// <summary>
/// Returns value of the variable stored in the system memory
/// </summary>
public class ValueGetDereferencedNode : DataNode
{
    public ValueGetDereferencedNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public override Value? GetValue(NodeContext? context = null)
    {
        if (Inputs.FirstOrDefault() == null)
        {
            return null;
        }
        Value? val = Inputs.FirstOrDefault()?.GetValue(context);
        if (val == null)
        {
            return null;
        }
        if (val.ValueType != ValueType.Address)
        {
            return null;
        }
        uint addr = (uint)(val?.Data ?? 0u);
        if (addr == 0)
        {
            throw new Interpreter.VisLangNullException("Attempted to dereference a value by address by address is NULL (address is 0)", this);
        }
        return Interpreter?.VisSystemMemory.Memory[addr];
    }
}