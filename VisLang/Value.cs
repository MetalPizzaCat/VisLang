namespace VisLang;
using VisLang.Interpreter;
public enum ValueType
{
    Bool,
    Char,
    Number,
    String,
    Address
}

public class Value
{
    public uint? Address { get; }
    public ValueType ValueType { get; set; } = ValueType.Bool;

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
            if (value == null)
            {
                _data = value;
                return;
            }
            switch (ValueType)
            {
                case ValueType.Bool:
                    if (value.GetType() != typeof(bool))
                    {
                        throw new Exception($"Value type mismatch. Expected bool got {value.GetType()}");
                    }
                    break;
                case ValueType.Char:
                    if (value.GetType() != typeof(char))
                    {
                        throw new Exception($"Value type mismatch. Expected char got {value.GetType()}");
                    }
                    break;
                case ValueType.Number:
                    if (value.GetType() != typeof(float))
                    {
                        throw new Exception($"Value type mismatch. Expected float got {value.GetType()}");
                    }
                    break;
                case ValueType.String:
                    if (value.GetType() != typeof(string))
                    {
                        throw new Exception($"Value type mismatch. Expected string got {value.GetType()}");
                    }
                    break;
            }
            _data = value;
        }
    }

    public Value(ValueType variableType, uint address, bool isArray, object? data)
    {
        ValueType = variableType;
        IsArray = isArray;
        _data = data;
        Address = address;
    }

    public Value(uint address)
    {
        Address = address;
    }


    public Value(ValueType variableType, bool isArray, object? data)
    {
        ValueType = variableType;
        IsArray = isArray;
        _data = data;
        Address = null;
    }

    public Value()
    {
        Address = null;
    }

    /// <summary>
    /// Compare two values<para/>
    /// Not overriding existing equality operators in c# because this function is only meant to be called from VisLang code
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public bool EqualsTo(Value value)
    {
        return ValueType == value.ValueType && (_data?.Equals(value._data) ?? false);
    }

    public string? AsString() => _data?.ToString();

    public float? AsNumber() => _data != null ? Convert.ToSingle(_data) : null;

    public bool? AsBool() => _data != null ? Convert.ToBoolean(_data) : null;

    public string TryAsString()
    {
        if (_data == null)
        {
            throw new NullReferenceException("Value data is null");
        }
        if (ValueType != ValueType.String)
        {
            throw new ValueTypeMismatchException($"Expected string got {ValueType.ToString()}");
        }
        return (string)_data;
    }

    public bool TryAsBool()
    {
        if (_data == null)
        {
            throw new NullReferenceException("Value data is null");
        }
        if (ValueType != ValueType.Bool)
        {
            throw new ValueTypeMismatchException($"Expected bool got {ValueType.ToString()}");
        }
        return ((bool?)_data).Value;
    }
}