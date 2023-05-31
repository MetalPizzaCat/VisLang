namespace VisLang;

public class VisCallable
{
    public VisCallable() { }

    public VisCallable(VisSystem mainSystem)
    {
        // pipe sub system output to parent system
        SubSystem.OnOutputAdded += mainSystem.AddOutput;
    }

    public string Name { get; set; } = "ProcedurePlaceHolderNameHereHelloGoodbye";

    /// <summary>
    /// All function variables that get their values from outside
    /// </summary>
    public Dictionary<string, ValueType> Arguments { get; set; } = new();

    /// <summary>
    /// All variables that are function is designed to have
    /// </summary>
    public Dictionary<string, ValueType> DefaultVariables { get; set; } = new();

    /// <summary>
    /// Special instance of VisSystem that will be executing all of the nodes. This is done to implement local variables
    /// </summary>
    public VisSystem SubSystem { get; set; } = new VisSystem();

    /// <summary>
    /// Type of the output value or null if procedure returns no value
    /// </summary>
    public ValueType? OutputValueType { get; set; }

    /// <summary>
    ///  Reset callable object to prepare for clean call
    /// </summary>
    public void Reset()
    {
        SubSystem.Reset();
        foreach ((string name, ValueType type) in DefaultVariables)
        {
            SubSystem.VisSystemMemory.CreateVariable(name, type);
        }
    }
}