namespace VisLang;

/// <summary>
/// Creates a dynamic variable that does not have an explicit name
/// </summary>
public class VariableAllocateNode : ExecutionNode
{
    public object DefaultValue = -1;

    public ValueType ValueType { get; set; } = ValueType.Bool;

    public object Value => Inputs.FirstOrDefault()?.GetValue()?.Data ?? DefaultValue;

    public uint? _createdVariableAddress = null;

    public VariableAllocateNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public override void Execute()
    {
        if (Interpreter == null)
        {
            throw new NullReferenceException("Interpreter system is null");
        }
        _createdVariableAddress = Interpreter?.VisSystemMemory.AllocateValue(ValueType, DefaultValue);
    }

    public override Value? GetValue()
    {
        return _createdVariableAddress.HasValue ? new Value(ValueType.Address, false, _createdVariableAddress) : null;
    }
}