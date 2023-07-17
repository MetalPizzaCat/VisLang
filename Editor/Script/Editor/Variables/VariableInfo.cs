namespace VisLang.Editor;
using System;

public class VariableInfo
{
    public VariableInfo(string name, VisLang.ValueType type, VisLang.ValueType? arrayDataType, bool array)
    {
        Id = new Guid();
        Name = name;
        Type = type;
        ArrayDataType = arrayDataType;
        IsArray = array;
    }

    public Guid Id { get; private set; }

    public string Name { get; set; }
    public VisLang.ValueType Type { get; set; }

    public VisLang.ValueType? ArrayDataType { get; set; } = null;
    public bool IsArray { get; set; } = false;
}