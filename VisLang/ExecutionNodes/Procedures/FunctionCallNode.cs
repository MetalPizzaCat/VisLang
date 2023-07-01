namespace VisLang;

/// <summary>
/// Retrieves data by calling the data node tree
/// </summary>
public class FunctionCallNode : ExecutionNode
{
    public FunctionCallNode() { }
    public FunctionCallNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public string? FunctionName { get; set; }

    private Value? _functionReturn = null;

    public override Value? GetValue()
    {
        if (Interpreter == null)
        {
            throw new NullReferenceException("Unable to perform function call because no system is available");
        }
        if (string.IsNullOrWhiteSpace(FunctionName))
        {
            throw new NullReferenceException("Unable to call function because no name was given");
        }
        VisFunction? proc = Interpreter.GetFunction(FunctionName);
        if (proc == null)
        {
            throw new NullReferenceException("Interpreter system does not contain a function with a given name");
        }

        VisSystem subSystem = proc.SubSystem;
        proc.Reset();

        // to pass arguments we also use a simple solution of just creating variables inside new system
        // this is done to ensure that we don't need to create special handling for when code is run from function
        // the reason why it is done inside caller instead of the function itself is so we could pass the argument values
        int currentArgumentId = 0;
        foreach ((string argName, ValueType argType) in proc.Arguments)
        {
            subSystem.VisSystemMemory.CreateVariable(argName, argType, Inputs.ElementAtOrDefault(currentArgumentId)?.GetValue()?.Data);
            // this solution could cause issues if arguments are messed up and don't match function signature
            // but will make editors and parsers deal with this problem :3
            currentArgumentId++;
        }
        // unlike function that need to run once and then have value referenced
        // data nodes calculate values in the moment of calling
        // as such there is no point in using @output system variable
        // and we can just return whatever value root has
        //
        // as a note: functions store first executable node while data stores last node because they traverse the tree in opposite directions
        return proc.Root?.GetValue();
    }
}