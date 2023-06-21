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
        MustMatchAll,
        /// <summary>
        /// Input can only be of the same type, but it does not matter if it's an array or not. Note: i have no idea where this would be useful
        /// </summary>
        MustMatchOnlyType,
        /// <summary>
        /// Input can be of any type but must be an array(if IsArray is true) or not array(if IsArray is false)
        /// </summary>
        MustMatchOnlyArray,
        /// <summary>
        /// Anything goes
        /// </summary>
        AllowAny
    }

    public FunctionInputInfo()
    {
    }

    public FunctionInputInfo(string inputName, ValueType inputType, bool isArray)
    {
        InputName = inputName;
        InputType = inputType;
        IsArray = isArray;
    }

    public FunctionInputInfo(VariableInfo info)
    {
        InputName = info.Name;
        InputType = info.ValueType;
        IsArray = info.IsArray;
    }

    [Export]
    public string InputName { get; set; } = "Default name";

    [Export]
    public VisLang.ValueType InputType { get; set; } = VisLang.ValueType.Bool;

    /// <summary>
    /// If true given input must be of array type
    /// </summary>
    [Export]
    public bool IsArray { get; set; } = false;

    /// <summary>
    /// If true any type can be passed into this input and InputType will be ignored
    /// </summary>
    [Export]
    public TypePermissions TypeMatchingPermissions { get; set; } = TypePermissions.MustMatchAll;
}