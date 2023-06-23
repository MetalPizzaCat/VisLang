namespace VisLang.Interpreter;

[System.Serializable]
public class InterpreterException : System.Exception
{
    protected VisNode? ExceptionSourceNode;
    /// <summary>
    /// Creates a new exception
    /// </summary>
    /// <param name="message">Message of the exception</param>
    /// <param name="node">Node that caused this exception to occur</param>
    /// <returns></returns>
    public InterpreterException(string message, VisNode? node) : base(message)
    {
        ExceptionSourceNode = node;
    }
    protected InterpreterException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
[System.Serializable]
public class MissingVariableException : InterpreterException
{
    public MissingVariableException(string message, VisNode? node) : base(message, node) { }
    protected MissingVariableException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}


[System.Serializable]
public class ValueTypeMismatchException : InterpreterException
{
    public ValueTypeMismatchException(string message, VisNode? node) : base(message, node) { }
    protected ValueTypeMismatchException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}