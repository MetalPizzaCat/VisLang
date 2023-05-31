namespace VisLang;

public class ProcedureCallNode : ExecutionNode
{
    public ProcedureCallNode() { }
    public ProcedureCallNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public string? ProcedureName { get; set; }

    private Value? _procedureReturn = null;

    public override void Execute()
    {
        base.Execute();
        if (Interpreter == null)
        {
            throw new NullReferenceException("Unable to perform procedure call because no system is available");
        }
        if (string.IsNullOrWhiteSpace(ProcedureName))
        {
            throw new NullReferenceException("Unable to call procedure because no name was given");
        }
        VisProcedure? proc = Interpreter.GetProcedure(ProcedureName);
        if (proc == null)
        {
            throw new NullReferenceException("Interpreter system does not contain a function with a given name");
        }
       
        VisSystem subSystem = proc.SubSystem;
        proc.Reset();

        // to pass arguments we also use a simple solution of just creating variables inside new system
        // this is done to ensure that we don't need to create special handling for when code is run from procedure
        // the reason why it is done inside caller instead of the Procedure itself is so we could pass the argument values
        int currentArgumentId = 0;
        foreach ((string argName, ValueType argType) in proc.Arguments)
        {
            subSystem.VisSystemMemory.CreateVariable(argName, argType, Inputs.ElementAtOrDefault(currentArgumentId)?.GetValue()?.Data);
            // this solution could cause issues if arguments are messed up and don't match function signature
            // but will make editors and parsers deal with this problem :3
            currentArgumentId++;
        }
        // a bit of a cheaty way to create variable returns 
        // technically this leaves the door open for functions that return multiple values
        // but since i'm trying to stick to c-ish style functions can only return one value
        //
        // If ANY procedure wants to have return value it WILL have to write into @output variable 
        if (proc.OutputValueType != null)
        {
            subSystem.VisSystemMemory.CreateVariable("@output", proc.OutputValueType.Value);
        }
        subSystem.Entrance = proc.ProcedureNodesRoot;
        subSystem.Execute();

        if (proc.OutputValueType != null)
        {
            _procedureReturn = subSystem.VisSystemMemory["@output"];
        }
    }
}