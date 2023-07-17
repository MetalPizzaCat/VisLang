using Godot;
using System;

[MonoCustomResourceRegistry.RegisteredType("CodeColorTheme")]
public partial class CodeColorTheme : Resource
{
    [ExportGroup("Type colors")]
    [Export]
    public Color AnyColor { get; set; }
    [Export]
    public Color BoolColor { get; set; }

    [Export]
    public Color CharColor { get; set; }

    [Export]
    public Color IntegerColor { get; set; }

    [Export]
    public Color FloatColor { get; set; }

    [Export]
    public Color StringColor { get; set; }

    [Export]
    public Color AddressColor { get; set; }

    [Export]
    public Color ArrayColor { get; set; }

    [ExportGroup("Node colors")]
    [Export]
    public Color FunctionColor { get; set; }

    [Export]
    public Color ProcedureColor { get; set; }

    /// <summary>
    /// Get color used for given type, if type is null AnyColor is returned
    /// </summary>
    /// <param name="type">Type or null if any type</param>
    /// <returns>Color that is defined in the theme</returns>
    public Color GetColorForType(VisLang.ValueType? type)
    {
        if (type == null)
        {
            return AnyColor;
        }
        switch (type.Value)
        {
            case VisLang.ValueType.Bool:
                return BoolColor;
            case VisLang.ValueType.Char:
                return CharColor;
            case VisLang.ValueType.Integer:
                return IntegerColor;
            case VisLang.ValueType.Float:
                return FloatColor;
            case VisLang.ValueType.String:
                return StringColor;
            case VisLang.ValueType.Address:
                return AddressColor;
            case VisLang.ValueType.Array:
                return ArrayColor;
            default:
                return new Color(1, 1, 1);
        }
    }
}
