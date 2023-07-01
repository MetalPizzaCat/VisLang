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

    /// <summary>
    ///  Returns value of the variable stored under the given name.<para></para>
    /// </summary>
    public Value? this[string name]
    {
        get
        {
            // we must have a record with this name
            if (!Variables.ContainsKey(name))
            {
                return null;
            }
            // and we must have an actual value in memory with this name
            if (!Memory.ContainsKey(Variables[name]))
            {
                return null;
            }
            return Memory[Variables[name]];
        }
    }

    public Value? GetValue(string name, Dictionary<string, uint>? context)
    {
        if (context == null)
        {
            // we must have a record with this name
            if (!Variables.ContainsKey(name))
            {
                return null;
            }
            // and we must have an actual value in memory with this name
            if (!Memory.ContainsKey(Variables[name]))
            {
                return null;
            }
            return Memory[Variables[name]];
        }
        else
        {
            // we must have a record with this name
            if (!context.ContainsKey(name))
            {
                return null;
            }
            // and we must have an actual value in memory with this name
            if (!Memory.ContainsKey(context[name]))
            {
                return null;
            }
            return Memory[context[name]];
        }
    }


    public uint AllocateValue(ValueType type, object? value = null, ValueType? arrayDataType = null)
    {
        Memory.Add(_memoryCounter, new Value(type, _memoryCounter, value, arrayDataType));
        _memoryCounter++;
        return _memoryCounter - 1;
    }

    /// <summary>
    /// Creates a new variable in the system and allocates memory for that variable.<para></para>
    /// This overload operates on a provided variable list. 
    /// </summary>
    /// <param name="variableList">Variable list that stores variable names, and where information about allocated value will be written to</param>
    /// <param name="name">Name of the variable</param>
    /// <param name="type">Type of the variable</param>
    /// <param name="value">Possible init value or null if default should be used</param>
    /// <param name="arrayDataType">If type is an array, this will define what type of data the array stores. Or null if array accepts anything</param>
    /// <returns>True if variable can be created or false if variable name is already taken</returns>
    public bool CreateVariable(ref Dictionary<string, uint> variableList, string name, ValueType type, object? value = null, ValueType? arrayDataType = null)
    {
        // this still works since all values are stored in the system even if variable names are not
        if (variableList.ContainsKey(name) && Memory.ContainsKey(variableList[name]))
        {
            return false;
        }
        uint addr = AllocateValue(type, value, arrayDataType);
        variableList.Add(name, addr);
        return true;
    }

    /// <summary>
    /// Creates a new variable in the system and allocates memory for that variable
    /// </summary>
    /// <param name="name">Name of the variable</param>
    /// <param name="type">Type of the variable</param>
    /// <param name="value">Possible init value or null if default should be used</param>
    /// <param name="arrayDataType">If type is an array, this will define what type of data the array stores. Or null if array accepts anything</param>
    /// <returns>True if variable can be created or false if variable name is already taken</returns>
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
    /// Removes whatever data was stored at the given address
    /// </summary>
    /// <param name="address"></param>
    public void FreeAddress(uint address)
    {
        Memory.Remove(address);
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

    public ExecutionNode? Current { get; set; } = null;

    public void AddOutput(string output)
    {
        OnOutputAdded?.Invoke(output);
        Output.Add(output);
    }

    /// <summary>
    /// Execute next item in the tree
    /// </summary>
    /// <returns>True if there is a next node that can be executed or false if no further nodes available</returns>
    public bool ExecuteNext(NodeContext? context)
    {
        if (Current == null)
        {
            return false;
        }
        Current.Execute(context);
        // second check here because some nodes might alter value of the Current(for example nodes that jump to a different set of instructions )
        if (Current == null)
        {
            return false;
        }
        Current = Current.GetNext();
        return Current != null;
    }

    /// <summary>
    /// Begins the execution from the first node and continues as long as there are nodes in the list
    /// </summary>
    public void Execute()
    {
        Entrance?.Execute();
        Current = Entrance?.GetNext();
        while (ExecuteNext(null) && Current != null)
        {
            Current.Execute();
            Current = Current.GetNext();
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