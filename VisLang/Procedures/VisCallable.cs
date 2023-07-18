namespace VisLang;


/// <summary>
/// Base object for all procedures and functions that can have it's own variables and can be called from elsewhere
/// </summary>
public class VisCallable
{
    public VisCallable() { }

    public string Name { get; set; } = "ProcedurePlaceHolderNameHereHelloGoodbye";

    /// <summary>
    /// All function variables that get their values from outside
    /// </summary>
    public Dictionary<string, ValueTypeData> Arguments { get; set; } = new();

    /// <summary>
    /// All variables that are function is designed to have
    /// </summary>
    public Dictionary<string, ValueTypeData> DefaultVariables { get; set; } = new();

    /// <summary>
    /// Type of the output value or null if procedure returns no value
    /// </summary>
    public ValueTypeData? OutputValueType { get; set; }

}