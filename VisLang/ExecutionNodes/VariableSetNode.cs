namespace VisLang;

public class VariableSetNode : ExecutionNode
{
    public string Name { get; set; } = "Default";

    public object DefaultValue = -1;

    public object GetInputValue(NodeContext? context) => Inputs.FirstOrDefault()?.GetValue(context)?.Data ?? DefaultValue;

    public VariableSetNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public VariableSetNode()
    {
    }

    public override void Execute(NodeContext? context = null)
    {
        VisSystem? sys = (context?.Interpreter ?? Interpreter);
        if (sys == null)
        {
            throw new NullReferenceException("Interpreter system is null");
        }
        Value? value = context?.Variables == null ? (sys.VisSystemMemory.GetValue(Name, null)) : (sys.VisSystemMemory.Memory[context.Variables[Name]]);

        if (value == null)
        {
            throw new Interpreter.MissingVariableException($"No variable with name {Name} found", this);
        }
        try
        {
            value.Data = GetInputValue(context);
        }
        catch (Interpreter.ValueTypeMismatchException e)
        {
            // yes i know, not the cleanest way but this way we can keep assignment as it is but still be able to tell user where it came from in code
            throw new Interpreter.ValueTypeMismatchException(e.Message, this);
        }
    }
}