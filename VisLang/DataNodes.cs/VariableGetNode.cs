namespace VisLang;

/// <summary>
/// Returns value of the variable stored in the system memory
/// </summary>
public class VariableGetNode : DataNode
{
    public VariableGetNode()
    {
    }

    public VariableGetNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public string Name { get; set; } = "Default";
    public override Value? GetValue()
    {
        return Interpreter?.VisSystemMemory[Name];
    }
}