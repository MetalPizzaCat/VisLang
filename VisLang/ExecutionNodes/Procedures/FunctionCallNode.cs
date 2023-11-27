using VisLang.Interpreter;

namespace VisLang;

/// <summary>
/// Retrieves data by calling the data node tree
/// </summary>
public class FunctionCallNode : DataNode
{
    public FunctionCallNode() { }
    public FunctionCallNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public string? FunctionName { get; set; }

    public override Value? GetValue(NodeContext? context = null)
    {
        if (Interpreter == null)
        {
            throw new VisLangNullException("Unable to perform function call because no system is available", this);
        }
        if (string.IsNullOrWhiteSpace(FunctionName))
        {
            throw new VisLangNullException("Unable to call function because no name was given", this);
        }
        VisFunction? proc = Interpreter.GetFunction(FunctionName);
        if (proc == null)
        {
            throw new VisLangNullException("Interpreter system does not contain a function with a given name", this);
        }
        Dictionary<string, uint> variables = new Dictionary<string, uint>();

        // to pass arguments we also use a simple solution of just creating variables inside new system
        // this is done to ensure that we don't need to create special handling for when code is run from function
        // the reason why it is done inside caller instead of the function itself is so we could pass the argument values
        int currentArgumentId = 0;
        foreach ((string argName, ValueTypeData argType) in proc.Arguments)
        {
            Interpreter.VisSystemMemory.CreateVariable(variables, argName, argType, Inputs.ElementAtOrDefault(currentArgumentId)?.GetValue(context)?.Data);
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
        return proc.Root?.GetValue(new NodeContext(Interpreter, variables));
    }
}