namespace VisLang;

public class VariableSetNode : ExecutionNode
{
    public string Name { get; set; } = "Default";

    public object DefaultValue = -1;

    public object Value => Inputs.FirstOrDefault()?.GetValue()?.Data ?? DefaultValue;

    public VariableSetNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public override void Execute()
    {
        if (Interpreter == null)
        {
            throw new NullReferenceException("Interpreter system is null");
        }
        if (Interpreter?.VisSystemMemory[Name] == null)
        {
            throw new Interpreter.MissingVariableException($"No variable with name {Name} found");
        }
        Interpreter.VisSystemMemory[Name].Data = Value;
    }
}