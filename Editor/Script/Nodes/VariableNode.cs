using Godot;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// A special node that does not need a FunctionInfo to be created and instead uses type of the variable
/// </summary>
public partial class VariableNode : VisNode
{
    protected class Variable
    {
        public Variable(string name, VisLang.ValueType valueType)
        {
            Name = name;
            ValueType = valueType;
        }

        public string Name { get; set; }
        public VisLang.ValueType ValueType { get; set; }
    }
    [Export]
    public OptionButton Variables { get; set; }

    protected List<Variable> Options = new();

    protected Variable? GetVariable(int option)
    {
        return Options.ElementAtOrDefault(option);
    }

    protected Variable? CurrentVariable => Options.ElementAtOrDefault(Variables.Selected);

    protected virtual FunctionInfo? GetFunctionInfo(Variable val) { return null; }

    public override void _Ready()
    {
        base._Ready();
        Variables.ItemSelected += VariableOptionSelected;
    }
    
    private void VariableOptionSelected(long option)
    {
        Variable? val = GetVariable((int)option);
        if (val != null)
        {
            FunctionInfo? info = GetFunctionInfo(val);
            if (info != null)
            {
                GenerateFunction(info);
            }
        }
    }

    public override void InitOnCanvas(MainScene canvas)
    {
        base.InitOnCanvas(canvas);
        if (canvas.VariableManager != null)
        {
            canvas.VariableManager.VariableListChanged += VariableListUpdated;
        }
    }

    /// <summary>
    /// Call this when list gets updated to update options available in this node
    /// </summary>
    /// <param name="variables">All variable available in the system</param>
    private void VariableListUpdated(List<VariableControl> variables)
    {
        Variables.Clear();
        Options.Clear();
        foreach (VariableControl control in variables)
        {
            Options.Add(new Variable(control.VariableName, control.VariableType));
            control.Changed += VariableUpdated;
            Variables.AddItem(control.VariableName);
        }
    }

    public void VariableUpdated(VariableControl? sender, string oldName, string? newName, VisLang.ValueType valType)
    {
        Variable? val = Options.FirstOrDefault(p => p.Name == oldName);
        if (val == null)
        {
            return;
        }
        if (newName != null)
        {
            val.Name = newName;
        }
        val.ValueType = valType;

        Variables.SetItemText(Options.IndexOf(val), newName);
    }
}