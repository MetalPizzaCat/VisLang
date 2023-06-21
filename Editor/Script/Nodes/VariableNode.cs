using Godot;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// A special node that does not need a FunctionInfo to be created and instead uses type of the variable
/// </summary>
public partial class VariableNode : EditorVisNode
{
    [Export]
    public Label VariableNameLabel { get; set; }

    protected List<VariableInfo> Options = new();

    public VariableInfo Info { get; set; }

    protected VariableInfo? GetVariable(int option)
    {
        return Options.ElementAtOrDefault(option);
    }

    protected virtual FunctionInfo? GetFunctionInfo(VariableInfo val) { return null; }

    public override void _Ready()
    {
        base._Ready();
    }

    public void InitControl(VariableInfo info)
    {
        Info = info;
        VariableNameLabel.Text = info.Name;
        FunctionInfo? fInfo = GetFunctionInfo(Info);
        if (fInfo != null)
        {
            GenerateFunction(fInfo);
        }
        info.NameChanged += VariableNameChanged;
        info.TypeChanged += VariableTypeChanged;
        info.ArrayChanged += VariableIsArrayChanged;
    }

    private void VariableNameChanged(VariableInfo? sender, string oldName, string newName)
    {
        VariableNameLabel.Text = newName;
    }

    private void VariableTypeChanged(VariableInfo? sender, VisLang.ValueType oldType, VisLang.ValueType newType)
    {
        FunctionInfo? info = GetFunctionInfo(new VariableInfo(Info.Name, newType, Info.IsArray));
        if (info != null)
        {
            GenerateFunction(info);
        }
    }

    private void VariableIsArrayChanged(VariableInfo? sender, bool oldValue, bool newValue)
    {
        FunctionInfo? info = GetFunctionInfo(new VariableInfo(Info.Name, Info.ValueType, newValue));
        if (info != null)
        {
            GenerateFunction(info);
        }
    }
}