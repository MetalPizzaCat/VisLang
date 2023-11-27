namespace VisLang.Editor;
using System;

public class VariableInfo
{
    public VariableInfo(string name, VisLang.ValueType type, VisLang.ValueType? arrayDataType, bool array)
    {
        Id = Guid.NewGuid();
        Name = name;
        Type = type;
        ArrayDataType = arrayDataType;
        IsArray = array;
    }

    public VariableInfo(string id, string name, VisLang.ValueType type, VisLang.ValueType? arrayDataType, bool isArray)
    {
        Id = new Guid(id);
        Name = name;
        Type = type;
        ArrayDataType = arrayDataType;
        IsArray = isArray;
    }

    public Guid Id { get; set; }

    public string Name { get; set; }
    public VisLang.ValueType Type { get; set; }

    public VisLang.ValueType? ArrayDataType { get; set; } = null;
    public bool IsArray { get; set; } = false;

    public VisLang.ValueTypeData FullType => new ValueTypeData(Type, ArrayDataType); 
}