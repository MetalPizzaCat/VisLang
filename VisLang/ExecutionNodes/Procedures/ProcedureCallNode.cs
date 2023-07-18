namespace VisLang;

public class ProcedureCallNode : ExecutionNode
{
    public ProcedureCallNode() { }
    public ProcedureCallNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public string? ProcedureName { get; set; }


    public override void Execute(NodeContext? context = null)
    {
        // if this function appears overcommented, then it's because i was writing algorithms in comments without code to not  forgoer what i was doing :3
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
        Dictionary<string, uint> variables = new Dictionary<string, uint>();

        // to pass arguments we also use a simple solution of just creating variables inside new system
        // this is done to ensure that we don't need to create special handling for when code is run from procedure
        // the reason why it is done inside caller instead of the Procedure itself is so we could pass the argument values
        int currentArgumentId = 0;
        foreach ((string argName, ValueTypeData argType) in proc.Arguments)
        {
            Interpreter.VisSystemMemory.CreateVariable(ref variables, argName, argType, Inputs.ElementAtOrDefault(currentArgumentId)?.GetValue(context)?.Data);
            // this solution could cause issues if arguments are messed up and don't match function signature
            // but will make editors and parsers deal with this problem :3
            currentArgumentId++;
        }
        foreach ((string argName, ValueTypeData argType) in proc.DefaultVariables)
        {
            Interpreter.VisSystemMemory.CreateVariable(ref variables, argName, argType, null);
        }
        // a bit of a cheaty way to create variable returns 
        // technically this leaves the door open for functions that return multiple values
        // but since i'm trying to stick to c-ish style functions can only return one value
        //
        // If ANY procedure wants to have return value it WILL have to write into @output variable 
        if (proc.OutputValueType != null)
        {
            Interpreter.VisSystemMemory.CreateVariable(ref variables, "@output", proc.OutputValueType.Value);
        }
        // TODO: Figure out how to point system to this node specifically
        // record where we were called from, like putting return address on the stack
        // this will technically be pointing to *this* node actually since this is what is happening
        ExecutionNode? callSource = this;
        // set new destination to function root
        Interpreter.Current = proc.ProcedureNodesRoot;
        // execute all given nodes
        while (Interpreter.ExecuteNext(new NodeContext(Interpreter, variables)))
        {
            // the whole thing is in the condition idk what do you want me to do here -_-
        }
        // store output(if we have one)
        if (proc.OutputValueType != null)
        {
            Interpreter.VisSystemMemory.FunctionReturnAddressesStack.Push(Interpreter.VisSystemMemory.Memory[variables["@output"]]);
        }
        // and clear the memory
        foreach ((string name, uint address) in variables)
        {
            Interpreter.VisSystemMemory.FreeAddress(address);
        }
        // if we set it back to callSource itself we will get stuck calling the same node until system dies
        Interpreter.Current = callSource?.DefaultNext;
    }
}