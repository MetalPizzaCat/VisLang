using Godot;
using VisLang;

/// <summary>
/// Special object storing information about function arguments. Has to be done this way to combat typed dictionaries not being supported
/// </summary>
[MonoCustomResourceRegistry.RegisteredType("FunctionInputInfo")]
public partial class FunctionInputInfo : Resource
{
    /// <summary>
    /// Type matching permissions for inputs
    /// </summary>
    public enum TypePermissions
    {
        /// <summary>
        /// Input can only be of the same type and be an array(if IsArray is true) or not array(if IsArray is false)
        /// </summary>
        SameTypeOnly,
        /// <summary>
        /// Anything goes
        /// </summary>
        AllowAny
    }

    public FunctionInputInfo()
    {
    }

    public FunctionInputInfo(string inputName, ValueType inputType, ValueType? arrayDataType)
    {
        InputName = inputName;
        InputType = inputType;
        ArrayDataType = arrayDataType ?? VisLang.ValueType.Bool;
        HasArrayType = arrayDataType != null;
    }

    public FunctionInputInfo(VariableInfo info)
    {
        InputName = info.Name;
        InputType = info.ValueType;
    }

    [Export]
    public string InputName { get; set; } = "Default name";

    [Export]
    public VisLang.ValueType InputType { get; set; } = VisLang.ValueType.Bool;
    /// <summary>
    /// If true any type can be passed into this input and InputType will be ignored
    /// </summary>
    [Export]
    public TypePermissions TypeMatchingPermissions { get; set; } = TypePermissions.SameTypeOnly;

    [ExportGroup("Arrays")]
    /// <summary>
    /// If true and is used in a function with IsArrayTypeDependent being true, type of this argument will be changed depending on the connected array
    /// </summary>
    /// <value></value>
    [Export]
    public bool IsArrayTypeDependent { get; set; } = false;

    [Export]
    public bool HasArrayType { get; set; } = false;
    [Export]
    public VisLang.ValueType ArrayDataType { get; set; } = VisLang.ValueType.Bool;
}