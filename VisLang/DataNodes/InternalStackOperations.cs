namespace VisLang.Internal;

/// <summary>
/// Retrieves top item from return values stack
/// Important: This *will* pop the item from stack, modifying the state of the app
/// </summary>
public class PopItemFromReturnStackNode : DataNode
{
    public PopItemFromReturnStackNode()
    {
    }

    public PopItemFromReturnStackNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    /// <summary>
    /// Gets value of the last item put on the function return stack <para><para/>
    /// Important: This *will* pop the item from stack, modifying the state of the app
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public override Value? GetValue(NodeContext? context)
    {

        Value? val = null;
        bool? success = Interpreter?.VisSystemMemory.FunctionReturnAddressesStack.TryPop(out val);
        if (!(success ?? false))
        {
            return null;
        }
        return val;
    }
}

/// <summary>
/// Returns value of the last return from function without removing it from the stack
/// </summary>
public class PeekAtItemFromReturnStackNode : DataNode
{
    public PeekAtItemFromReturnStackNode()
    {
    }

    public PeekAtItemFromReturnStackNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    /// <summary>
    /// Gets value of the last item put on the function return stack <para><para/>
    /// </summary>
    /// <param name="context"></param>
    public override Value? GetValue(NodeContext? context)
    {
        Value? val = null;
        bool? success = Interpreter?.VisSystemMemory.FunctionReturnAddressesStack.TryPeek(out val);
        if (!(success ?? false))
        {
            return null;
        }
        return val;
    }
}