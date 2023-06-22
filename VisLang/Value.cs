namespace VisLang;
using VisLang.Interpreter;

/// <summary>
///  All possible base value types in the VisLang
/// </summary>
public enum ValueType
{
    /// <summary>
    /// True or false value. Uses c# bool
    /// </summary>
    Bool,
    /// <summary>
    /// Single character. Uses c# char
    /// </summary>
    Char,
    /// <summary>
    /// Integer number for indexing arrays and loops. Uses c# int
    /// </summary>
    Integer,
    /// <summary>
    /// Floating point number. Uses c# float
    /// </summary>
    Float,
    /// <summary>
    /// String of characters. Uses c# string
    /// </summary>
    String,
    /// <summary>
    /// Address of the value in the interpreter memory. Uses c# uint
    /// </summary>
    Address,
    Array,
}

/// <summary>
/// Special object used for representing data in the VisLang memory
/// </summary>
public class Value
{
    /// <summary>
    /// Address of the variable in the interpreter memory list or null if value is not recorded in the memory list
    /// </summary>
    public uint? Address { get; }
    /// <summary>
    /// Current type of the value
    /// </summary>
    public ValueType ValueType { get; set; } = ValueType.Bool;

    public bool IsArray => ValueType == ValueType.Array;

    /// <summary>
    /// The actual data stored in the value
    /// </summary>
    private object? _data = null;

    /// <summary>
    /// The actual data object stored in the value. When setting value type must match type in ValueType 
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
                        throw new ValueTypeMismatchException($"Value type mismatch. Expected bool got {value.GetType()}");
                    }
                    break;
                case ValueType.Char:
                    if (value.GetType() != typeof(char))
                    {
                        throw new ValueTypeMismatchException($"Value type mismatch. Expected char got {value.GetType()}");
                    }
                    break;
                case ValueType.Integer:
                    if (value.GetType() != typeof(float))
                    {
                        throw new ValueTypeMismatchException($"Value type mismatch. Expected int got {value.GetType()}");
                    }
                    break;
                case ValueType.Float:
                    if (value.GetType() != typeof(float))
                    {
                        throw new ValueTypeMismatchException($"Value type mismatch. Expected float got {value.GetType()}");
                    }
                    break;
                case ValueType.String:
                    if (value.GetType() != typeof(string))
                    {
                        throw new ValueTypeMismatchException($"Value type mismatch. Expected string got {value.GetType()}");
                    }
                    break;
                case ValueType.Address:
                    if (value.GetType() != typeof(uint))
                    {
                        throw new ValueTypeMismatchException($"Value type mismatch. Expected uint got {value.GetType()}");
                    }
                    break;
                case ValueType.Array:
                    if (value.GetType() != typeof(List<Value>))
                    {
                        throw new ValueTypeMismatchException($"Value type mismatch. Expected array got {value.GetType()}");
                    }
                    break;
            }
            _data = value;
        }
    }
    /// <summary>
    /// Creates a new instance of value object of given type, with address of where it is stored in the memory
    /// </summary>
    /// <param name="variableType">What data does this value store</param>
    /// <param name="address"> Address of the variable in the memory</param>
    /// <param name="isArray">If true value will be an array that uses List</param>
    /// <param name="data">Possible init data or null if no data is needed.</param>
    public Value(ValueType variableType, uint address, object? data)
    {
        ValueType = variableType;
        if (IsArray)
        {
            // arrays are stored as list of value objects
            // while technically arrays in the language can only store one object type
            // it becomes very annoying to have to deal with keeping track of all allowed types 
            // and performing constant conversions
            _data = new List<Value>();
        }
        else
        {
            _data = data;
        }
        Address = address;
    }

    /// <summary>
    /// Creates a new instance opf value of given type without any record of it being written to the interpreter variable memory list
    /// </summary>
    /// <param name="variableType">What data does this value store</param>
    /// <param name="isArray">If true value will be an array that uses List</param>
    /// <param name="data">Possible init data or null if no data is needed.</param>
    public Value(ValueType variableType, object? data)
    {
        ValueType = variableType;
        if (IsArray && data == null)
        {
            // arrays are stored as list of value objects
            // while technically arrays in the language can only store one object type
            // it becomes very annoying to have to deal with keeping track of all allowed types 
            // and performing constant conversions
            _data = new List<Value>();
        }
        else
        {
            _data = data;
        }
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

    /// <summary>
    /// Gets string value for the data stored in the object using `.ToString`. This is meant for simplifying internal conversions and does NOT perform type checks 
    /// </summary>
    public string? AsString() => (IsArray ? string.Join(',', _data) : _data?.ToString());

    /// <summary>
    ///Tries to convert value stored in object to float. This is meant for simplifying internal conversions and does NOT perform type checks 
    /// </summary>
    public int? AsInteger() => _data != null ? Convert.ToInt32(_data) : null;

    /// <summary>
    ///Tries to convert value stored in object to float. This is meant for simplifying internal conversions and does NOT perform type checks 
    /// </summary>
    public float? AsFloat() => _data != null ? Convert.ToSingle(_data) : null;

    /// <summary>
    /// Tries to convert value stored in object to bool. This is meant for simplifying internal conversions and does NOT perform type checks 
    /// </summary>
    public bool? AsBool() => _data != null ? Convert.ToBoolean(_data) : null;

    /// <summary>
    /// Convert object to string. This will convert any value to string and will only fail if value is null. Arrays will be converted to [a1,a2,...an] string.
    /// 
    /// Can not cause ValueTypeMismatchException
    /// </summary>
    public string TryAsString()
    {
        if (_data == null)
        {
            throw new NullReferenceException("Value data is null");
        }
        if (IsArray)
        {
            return $"[{string.Join(',', (_data as List<Value>).Select(i => i.IsArray ? i.TryAsString() : i.Data))}]";
        }
        return _data?.ToString() ?? "you some how managed to bypass null check in TryAsString(), fascinating. You get a cookie :3";
    }

    public int TryAsInt()
    {
        if (_data == null)
        {
            throw new NullReferenceException("Value data is null");
        }
        if (ValueType != ValueType.Integer)
        {
            throw new ValueTypeMismatchException($"Expected int got {ValueType.ToString()}");
        }
        return ((int?)_data).Value;
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