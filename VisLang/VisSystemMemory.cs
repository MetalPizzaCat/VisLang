using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;

namespace VisLang;

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
    /// The stack where functions can puts their return values, for other nodes to grab from<para></para>
    /// This is alternative to storing node results in the node itself. This also in theory allows for multiple return values
    /// </summary>
    public Stack<Value> FunctionReturnAddressesStack { get; set; } = new();

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


    public uint AllocateValue(ValueTypeData type, object? value = null)
    {
        Memory.Add(_memoryCounter, new Value(type, _memoryCounter, value));
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
    /// <returns>True if variable can be created or false if variable name is already taken</returns>
    public bool CreateVariable(ref Dictionary<string, uint> variableList, string name, ValueTypeData type, object? value = null)
    {
        // this still works since all values are stored in the system even if variable names are not
        if (variableList.ContainsKey(name) && Memory.ContainsKey(variableList[name]))
        {
            return false;
        }
        uint addr = AllocateValue(type, value);
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
    public bool CreateVariable(string name, ValueTypeData type, object? value = null)
    {
        if (this[name] != null)
        {
            return false;
        }
        uint addr = AllocateValue(type, value);
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
