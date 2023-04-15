namespace VisLang.Interpreter;

[System.Serializable]
public class MissingVariableException : System.Exception
{
    public MissingVariableException() { }
    public MissingVariableException(string message) : base(message) { }
    public MissingVariableException(string message, System.Exception inner) : base(message, inner) { }
    protected MissingVariableException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}