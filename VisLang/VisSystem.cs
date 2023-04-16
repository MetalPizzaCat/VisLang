using System.Collections.Generic;
using System.Linq;

namespace VisLang;

public class Variable
{
    public Variable(string name, Value value)
    {
        Name = name;
        Value = value;
    }

    public string Name { get; set; } = String.Empty;

    public Value Value { get; set; } = new();
}

public class VisSystemMemory
{
    public List<Variable> Variables { get; set; } = new();

    public Variable? this[string name] => Variables.FirstOrDefault(p => p.Name == name);

    public bool CreateVariable(string name, ValueType type, object? value = null)
    {
        if (this[name] != null)
        {
            return false;
        }
        Variables.Add(new Variable(name, new Value(type, false, value)));
        return true;
    }
}

/// <summary>
/// Class responsible for storing and handling all of the information related to language execution
/// </summary>
public class VisSystem
{
    public List<ExecutionNode> Code { get; set; } = new();

    public VisSystemMemory VisSystemMemory { get; set; } = new();
    /// <summary>
    /// Output produced during the execution by Execution nodes
    /// </summary>
    /// <returns></returns>
    public List<string> Output { get; set; } = new();

    /// <summary>
    /// Node from which the execution starts
    /// </summary>
    /// <value></value>
    public ExecutionNode? Entrance { get; set; } = null;

    public void AddOutput(string output)
    {
        Output.Add(output);
    }

    public void Execute()
    {
        Entrance?.Execute();
        ExecutionNode? next = Entrance?.GetNext();
        while (next != null)
        {
            next.Execute();
            next = next.GetNext();
        }
    }
}