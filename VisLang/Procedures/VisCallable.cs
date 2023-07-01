namespace VisLang;

/// <summary>
/// Callable represents a function(like the ones in C or C#) which can be executed and have return values. <para></para>
/// Callables work by creating a sub interpreter system(or using provided one) and executing their code inside it, recording the output(if present) after the execution<para></para>
/// Unlike VisFunction which is meant only as a way to store collections of DataNodes, Callables can store both executable and data nodes, as they act as program inside program
/// </summary>
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