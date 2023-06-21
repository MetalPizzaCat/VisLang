/// <summary>
/// Special class for storing variable info for UI purposes that will send events when properties change
/// </summary>
public class VariableInfo
{
    public delegate void NameChangedEventHandler(VariableInfo? sender, string oldName, string newName);
    public delegate void TypeChangedEventHandler(VariableInfo? info, VisLang.ValueType oldType, VisLang.ValueType newType);
    public delegate void ArrayChangedEventHandler(VariableInfo? info, bool oldValue, bool newValue);

    /// <summary>
    /// Called before name change is applied
    /// </summary>
    public event NameChangedEventHandler? NameChanged;
    public event TypeChangedEventHandler? TypeChanged;
    public event ArrayChangedEventHandler? ArrayChanged;
    public VariableInfo(string name, VisLang.ValueType valueType, bool isArray)
    {
        _name = name;
        _type = valueType;
        _array = isArray;
    }

    private string _name;
    private VisLang.ValueType _type;
    private bool _array;

    public string Name
    {
        get => _name;
        set
        {
            NameChanged?.Invoke(this, _name, value);
            _name = value;
        }
    }

    public VisLang.ValueType ValueType
    {
        get => _type;
        set
        {
            TypeChanged?.Invoke(this, _type, value);
            _type = value;
        }
    }

    public bool IsArray
    {
        get => _array;
        set
        {
            ArrayChanged?.Invoke(this, _array, value);
            _array = value;
        }
    }
}