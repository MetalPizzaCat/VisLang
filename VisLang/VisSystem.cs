using System.Collections.Generic;
using System.Linq;

namespace VisLang;

public enum VariableType
{
    Bool,
    Char,
    Number,
    String
}

public class Value
{
    public VariableType VariableType { get; set; } = VariableType.Bool;

    public bool IsArray { get; set; } = false;

    /// <summary>
    /// The actual data stored in the value
    /// </summary>
    private object? _data = null;

    /// <summary>
    /// The actual data stored in the value
    /// </summary>
    public object? Data
    {
        get => _data;
        set
        {
            switch (VariableType)
            {
                case VariableType.Bool:
                    if (value.GetType() != typeof(bool))
                    {
                        throw new Exception($"Value type mismatch. Expected bool got {value.GetType()}");
                    }
                    break;
                case VariableType.Char:
                    if (value.GetType() != typeof(char))
                    {
                        throw new Exception($"Value type mismatch. Expected char got {value.GetType()}");
                    }
                    break;
                case VariableType.Number:
                    if (value.GetType() != typeof(float))
                    {
                        throw new Exception($"Value type mismatch. Expected float got {value.GetType()}");
                    }
                    break;
                case VariableType.String:
                    if (value.GetType() != typeof(string))
                    {
                        throw new Exception($"Value type mismatch. Expected string got {value.GetType()}");
                    }
                    break;
            }
            _data = value;
        }
    }

    public Value(VariableType variableType, bool isArray, object? data)
    {
        VariableType = variableType;
        IsArray = isArray;
        _data = data;
    }

    public Value()
    {
    }

    public string? AsString() => _data?.ToString();

    public float? AsNumber() => _data != null ? Convert.ToSingle(_data) : null;
}

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

    public bool CreateVariable(string name, VariableType type, object? value = null)
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
        ExecutionNode? next = Entrance?.Next;
        while (next != null)
        {
            next.Execute();
            next = next.Next;
        }
    }
}