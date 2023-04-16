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


[System.Serializable]
public class ValueTypeMismatchException : System.Exception
{
    public ValueTypeMismatchException() { }
    public ValueTypeMismatchException(string message) : base(message) { }
    public ValueTypeMismatchException(string message, System.Exception inner) : base(message, inner) { }
    protected ValueTypeMismatchException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}