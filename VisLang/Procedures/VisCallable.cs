namespace VisLang;


public class VisCallable
{
    public VisCallable() { }

    public string Name { get; set; } = "ProcedurePlaceHolderNameHereHelloGoodbye";

    /// <summary>
    /// All function variables that get their values from outside
    /// </summary>
    public Dictionary<string, ValueType> Arguments { get; set; } = new();

    /// <summary>
    /// All variables that are function is designed to have
    /// </summary>
    public Dictionary<string, ValueType> DefaultVariables { get; set; } = new();

    public Dictionary<string, uint> Variables { get; set; } = new Dictionary<string, uint>();

    /// <summary>
    /// Type of the output value or null if procedure returns no value
    /// </summary>
    public ValueType? OutputValueType { get; set; }

}