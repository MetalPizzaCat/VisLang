using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

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

    public uint AllocateValue(ValueType type, object? value = null, ValueType? arrayDataType = null)
    {
        Memory.Add(_memoryCounter, new Value(type, _memoryCounter, value, arrayDataType));
        _memoryCounter++;
        return _memoryCounter - 1;
    }

    public bool CreateVariable(string name, ValueType type, object? value = null, ValueType? arrayDataType = null)
    {
        if (this[name] != null)
        {
            return false;
        }
        uint addr = AllocateValue(type, value, arrayDataType);
        Variables.Add(name, addr);
        return true;
    }

    /// <summary>
    /// Resets all storage and counters to default values. Does not actually check if data is in use or if data is actually destroyed
    /// </summary>
    public void Clear()
    {
        Memory.Clear();
        Variables.Clear();
        _memoryCounter = 1;
    }
}

/// <summary>
/// Class responsible for storing and handling all of the information related to language execution
/// </summary>
public class VisSystem
{
    public delegate void OutputAddedEventHandler(string output);
    public event OutputAddedEventHandler? OnOutputAdded;

    public List<ExecutionNode> Code { get; set; } = new();

    public VisSystemMemory VisSystemMemory { get; set; } = new();

    /// <summary>
    /// All of the procedures loaded in the memory that can be referenced by nodes inside this system
    /// </summary>
    /// <returns></returns>
    public List<VisProcedure> Procedures { get; set; } = new();

    public VisProcedure? GetProcedure(string name) => Procedures.FirstOrDefault(p => p.Name == name);

    public List<VisFunction> Functions { get; set; } = new();

    public VisFunction? GetFunction(string name) => Functions.FirstOrDefault(p => p.Name == name);

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
        OnOutputAdded?.Invoke(output);
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

    /// <summary>
    /// Clears all of the functions and variables from system
    /// </summary>
    public void Reset()
    {
        VisSystemMemory.Clear();
    }
}