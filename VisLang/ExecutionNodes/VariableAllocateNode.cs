namespace VisLang;

/// <summary>
/// Creates a dynamic variable that does not have an explicit name
/// </summary>
public class VariableAllocateNode : ExecutionNode
{
    public object DefaultValue = -1;

    public ValueType ValueType { get; set; } = ValueType.Bool;

    public object GetInputValue(NodeContext? context) => Inputs.FirstOrDefault()?.GetValue(context)?.Data ?? DefaultValue;


    public VariableAllocateNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public override void Execute(NodeContext? context = null)
    {
        if (Interpreter == null)
        {
            throw new NullReferenceException("Interpreter system is null");
        }
        Interpreter?.VisSystemMemory.FunctionReturnAddressesStack.Push(new Value(ValueType.Address, Interpreter.VisSystemMemory.AllocateValue(ValueType, DefaultValue)));
    }
}