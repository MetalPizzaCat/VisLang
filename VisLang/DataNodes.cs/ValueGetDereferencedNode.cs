namespace VisLang;

/// <summary>
/// Returns value of the variable stored in the system memory
/// </summary>
public class ValueGetDereferencedNode : DataNode
{
    public ValueGetDereferencedNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public override Value? GetValue()
    {
        // if (Inputs.FirstOrDefault() == null || Inputs.FirstOrDefault().GetValue() == null || !Inputs.FirstOrDefault().GetValue().Address.HasValue)
        // {
        //     return null;
        // }

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
        return Interpreter?.VisSystemMemory.Memory[(uint)Inputs.FirstOrDefault().GetValue().Data];
    }
}