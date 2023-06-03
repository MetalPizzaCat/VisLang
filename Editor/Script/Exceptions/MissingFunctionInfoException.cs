[System.Serializable]
public class MissingFunctionInfoException : System.Exception
{
    public MissingFunctionInfoException() { }
    public MissingFunctionInfoException(string message) : base(message) { }
    public MissingFunctionInfoException(string message, System.Exception inner) : base(message, inner) { }
    protected MissingFunctionInfoException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}