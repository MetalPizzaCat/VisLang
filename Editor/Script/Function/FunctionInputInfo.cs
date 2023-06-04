using Godot;
using VisLang;

/// <summary>
/// Special object storing information about function arguments. Has to be done this way to combat typed dictionaries not being supported
/// </summary>
[MonoCustomResourceRegistry.RegisteredType("FunctionInputInfo")]
public partial class FunctionInputInfo : Resource
{
    public FunctionInputInfo()
    {
    }

    public FunctionInputInfo(string inputName, ValueType inputType)
    {
        InputName = inputName;
        InputType = inputType;
    }

    [Export]
    public string InputName { get; set; } = "Default name";

    [Export]
    public VisLang.ValueType InputType { get; set; } = VisLang.ValueType.Bool;

    /// <summary>
    /// If true any type can be passed into this input and InputType will be ignored
    /// </summary>
    [Export]
    public bool AllowAnyType { get; set; } = false;
}