using Godot;
using System;

public partial class VariableControl : HBoxContainer
{
    public delegate void ChangedEventHandler(VariableControl? sender, string name, string newName, VisLang.ValueType valueType);
    public event ChangedEventHandler? Changed;

    [Export]
    public LineEdit NameEdit { get; set; }

    [Export]
    public OptionButton TypeOptionButton { get; set; }

    private string _oldName = string.Empty;

    public string VariableName => NameEdit.Text;

    public VisLang.ValueType VariableType => TypeOptionButton.Selected > -1 ? (VisLang.ValueType)TypeOptionButton.Selected : VisLang.ValueType.Bool;

    public override void _Ready()
    {
        foreach (VisLang.ValueType val in (VisLang.ValueType[])Enum.GetValues(typeof(VisLang.ValueType)))
        {
            TypeOptionButton.AddItem(val.ToString());
        }
    }

    private void ChangeName(string newName)
    {
        Changed?.Invoke(this, _oldName, newName, VariableType);
    }

    private void SelectType(int type)
    {
        Changed?.Invoke(this, _oldName, null, VariableType);
    }
}
