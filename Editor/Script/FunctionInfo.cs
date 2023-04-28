using System;
using VisLang;
using System.Collections.Generic;


public class FunctionInfo
{
    /// <summary>
    /// Is this executable or data node 
    /// </summary>
    public bool IsExecutable { get; set; } = false;

    public Dictionary<string, VisLang.ValueType> Inputs = new Dictionary<string, VisLang.ValueType>();

    public VisLang.ValueType? Output = null;

    public FunctionInfo(bool isExecutable, Dictionary<string, VisLang.ValueType> inputs, VisLang.ValueType? output)
    {
        IsExecutable = isExecutable;
        Inputs = inputs;
        Output = output;
    }

    public FunctionInfo()
    {
    }
}