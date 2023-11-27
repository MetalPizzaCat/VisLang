namespace VisLang;

/// <summary>
/// Base node for all "for" based loops
/// </summary>
public class ForNode : ForNodeBase
{
    public ForNode()
    {
    }

    public ForNode(VisSystem? interpreter) : base(interpreter)
    {

    }

    public Value GetStartValue(NodeContext? context) => Inputs.FirstOrDefault()?.GetValue(context) ?? new Value(new ValueTypeData(ValueType.Integer), 0L);
    public Value GetStopValue(NodeContext? context) => Inputs.ElementAtOrDefault(1)?.GetValue(context) ?? new Value(new ValueTypeData(ValueType.Integer), 0L);
    public Value GetStepValue(NodeContext? context) => Inputs.ElementAtOrDefault(2)?.GetValue(context) ?? new Value(new ValueTypeData(ValueType.Integer), 0L);

    public override void Execute(NodeContext? context = null)
    {
        base.Execute(context);
        if (Interpreter == null)
        {
            throw new Interpreter.VisLangNullException("Interpreter is invalid", this);
        }
        Value? val = Interpreter?.VisSystemMemory.GetValue(IteratorVariableName, context?.Variables);
        if (val == null)
        {
            // skip execution of a loop that starts and ends with the same value
            if (GetStartValue(context).Data == GetStopValue(context))
            {
                FinishLoop(context);
                return;
            }
            // if we don't have our iterator variable created we assume that we are starting the iteration raw
            // other solution could be to reset this flag elsewhere but this requires understanding whether we already finished execution or not
            WasFinished = false;
            if (context == null)
            {
                Interpreter?.VisSystemMemory.CreateVariable(IteratorVariableName, GetStartValue(context).TypeData, GetStartValue(context).Data);
            }
            else
            {
                Interpreter?.VisSystemMemory.CreateVariable(context.Variables, IteratorVariableName, GetStartValue(context).TypeData, GetStartValue(context).Data);
            }
            
            return;
        }
        if (val.ValueType == ValueType.Integer)
        {
            val.Data = (long)(val.Data ?? 0) + (long)(GetStepValue(context).Data ?? 0L);
            if ((long)val.Data >= (long)(GetStopValue(context).Data ?? 0L))
            {
                FinishLoop(context);
            }
        }
        if (val.ValueType == ValueType.Float)
        {
            val.Data = (double)(val.Data ?? 0.0) + (double)(GetStepValue(context).Data ?? 0.0);
            if ((double)val.Data >= (double)(GetStopValue(context).Data ?? 0.0))
            {
                FinishLoop(context);
            }
        }
    }

}