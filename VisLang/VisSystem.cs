using System.Collections.Generic;
using System.Linq;

namespace VisLang;

public class Variable
{
    public Variable(string name, uint address)
    {
        Name = name;
        Address = address;
    }

    public string Name { get; set; } = String.Empty;

    public uint Address { get; set; } = 0;
}

public class VisSystemMemory
{

    /// <summary>
    /// Address of the slot where next allocated value will be put<para/>
    /// Starts with 1 because like in C 0 means null
    /// </summary>
    private uint _memoryCounter = 1;
    /// <summary>
    /// All of the allocated variables in the system
    /// </summary>
    public Dictionary<uint, Value> Memory { get; set; } = new();

    /// <summary>
    /// All of the named variables<para/>
    /// * Key - Variable name<para/>
    /// * Value - Variable address in Memory collection
    /// </summary>
    public Dictionary<string, uint> Variables { get; set; } = new();

    public Value? this[string name] => Variables.ContainsKey(name) ? (Memory.ContainsKey(Variables[name]) ? Memory[Variables[name]] : null) : null;

    public uint AllocateValue(ValueType type, object? value = null)
    {
        Memory.Add(_memoryCounter, new Value(type, _memoryCounter, false, value));
        _memoryCounter++;
        return _memoryCounter - 1;
    }

    public bool CreateVariable(string name, ValueType type, object? value = null)
    {
        if (this[name] != null)
        {
            return false;
        }
        uint addr = AllocateValue(type, value);
        Variables.Add(name, addr);
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