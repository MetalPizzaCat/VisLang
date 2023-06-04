[System.Serializable]
public class VisLangVisualParserException : System.Exception
{
    public VisLangVisualParserException() { }
    public VisLangVisualParserException(string message) : base(message) { }
    public VisLangVisualParserException(string message, System.Exception inner) : base(message, inner) { }
    protected VisLangVisualParserException(
        System.Runtime.Serialization.SerializationInfo info,
        System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}