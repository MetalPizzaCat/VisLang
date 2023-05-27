using Godot;
using System;

public partial class VariableControl : HBoxContainer
{
    [Export]
    public LineEdit NameEdit { get; set; }

    [Export]
    public OptionButton TypeOptionButton { get; set; }

    public string VariableName => NameEdit.Text;

    public VisLang.ValueType VariableType => TypeOptionButton.Selected > -1 ? (VisLang.ValueType)TypeOptionButton.Selected : VisLang.ValueType.Bool;

    public override void _Ready()
    {
        foreach (VisLang.ValueType val in (VisLang.ValueType[])Enum.GetValues(typeof(VisLang.ValueType)))
        {
            TypeOptionButton.AddItem(val.ToString());
        }
    }
}
