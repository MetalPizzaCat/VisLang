namespace VisLang;

public class VariableSetNode : ExecutionNode
{
    public string Name { get; set; } = "Default";

    public object DefaultValue = -1;

    public object Value => Inputs.FirstOrDefault()?.GetValue()?.Data ?? DefaultValue;

    public VariableSetNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public VariableSetNode()
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
            throw new Interpreter.MissingVariableException($"No variable with name {Name} found", this);
        }
        try
        {
            Interpreter.VisSystemMemory[Name].Data = Value;
        }
        catch (Interpreter.ValueTypeMismatchException e)
        {
            // yes i know, not the cleanest way but this way we can keep assignment as it is but still be able to tell user where it came from in code
            throw new Interpreter.ValueTypeMismatchException(e.Message, this);
        }
    }
}