/// <summary>
/// Special class for storing variable info for UI purposes that will send events when properties change
/// </summary>
public class VariableInfo
{
    public delegate void NameChangedEventHandler(VariableInfo? sender, string oldName, string newName);
    public delegate void TypeChangedEventHandler(VariableInfo? info, VisLang.ValueType oldType, VisLang.ValueType newType);

    /// <summary>
    /// Called before name change is applied
    /// </summary>
    public event NameChangedEventHandler? NameChanged;
    public event TypeChangedEventHandler? TypeChanged;
    public VariableInfo(string name, VisLang.ValueType valueType)
    {
        _name = name;
        _type = valueType;
    }

    private string _name;
    private VisLang.ValueType _type;

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
}