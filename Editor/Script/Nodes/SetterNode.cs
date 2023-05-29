using Godot;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// A special execution node that does not need a FunctionInfo to be created and instead uses type of the variable
/// </summary>
public partial class SetterNode : VisNode
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

    private List<Variable> options = new();

    protected Variable? CurrentVariable => options.ElementAtOrDefault(Variables.Selected);

    private void VariableOptionSelected(int option)
    {
        Variable? val = options.ElementAtOrDefault(option);
        if (val == null)
        {
            return;
        }
        GenerateFunction(new FunctionInfo(true, new Dictionary<string, VisLang.ValueType>()
        {
             {"Value", val.ValueType}
        }, null, "VisLang.VariableSetNode", "Set"));
    }

    /// <summary>
    /// Call this when list gets updated to update options available in this node
    /// </summary>
    /// <param name="variables">All variable available in the system</param>
    public void VariableListUpdated(List<VariableControl> variables)
    {
        Variables.Clear();
        options.Clear();
        foreach (VariableControl control in variables)
        {
            options.Add(new Variable(control.VariableName, control.VariableType));
            control.Changed += VariableUpdated;
            Variables.AddItem(control.VariableName);
        }
    }

    public void VariableUpdated(VariableControl? sender, string oldName, string? newName, VisLang.ValueType valType)
    {
        Variable? val = options.FirstOrDefault(p => p.Name == oldName);
        if (val == null)
        {
            return;
        }
        if (newName != null)
        {
            val.Name = newName;
        }
        val.ValueType = valType;

        Variables.SetItemText(options.IndexOf(val), newName);
    }

    protected override void ApplyAdditionalDataToNode<NodeType>(NodeType node)
    {
        base.ApplyAdditionalDataToNode(node);
        if(node is VisLang.VariableSetNode set && CurrentVariable != null)
        {
            set.Name = CurrentVariable.Name;
        }
    }
}