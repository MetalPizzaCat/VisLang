namespace VisLang.Editor;
using System.Collections.Generic;
using VisLang.Editor.Files;

public partial class EditorGraphVariableNode : EditorGraphNode
{
    private VariableInfo _variable;
    public bool IsGetter { get; set; } = true;

    public string VariableName
    {
        get => _variable.Name;
        set
        {
            // there is no point in fully recreating the node as name itself does not have any effect on the node function
            _variable.Name = value;
            Title = IsGetter ? $"Get: {value}" : $"Set: {value}";
        }
    }
    public VariableInfo Variable
    {
        get => _variable;
        set
        {
            _variable = value;
            if (IsGetter)
            {
                GenerateFunction(new FunctionInfo
                            (
                                $"Get: {_variable.Name}",
                                false,
                                new(),
                                "VisLang.VariableGetNode",
                                true,
                                _variable.Type,
                                true,
                                _variable.ArrayDataType != null,
                                _variable.ArrayDataType ?? ValueType.Bool
                            ));
            }
            else
            {
                GenerateFunction(new FunctionInfo
                                    (
                                        $"Set: {_variable.Name}",
                                        true,
                                        new() { new FunctionInputInfo("Value", _variable.Type, _variable.ArrayDataType) },
                                        "VisLang.VariableSetNode",
                                        false
                                    ));
            }

        }
    }

    protected override void ApplyAdditionalCreationData<NodeType>(NodeType node)
    {
        base.ApplyAdditionalCreationData(node);
        if (node is VariableSetNode setter)
        {
            setter.Name = Variable.Name;
        }
        else if (node is VariableGetNode getter)
        {
            getter.Name = Variable.Name;
        }
    }

    public Files.VariableNodeSaveData GetVariableSaveData()
    {
        Files.EditorNodeSaveData data = GetSaveData();
        return new Files.VariableNodeSaveData(Variable.Id, IsGetter, Name, GlobalPosition)
        {
            ManualInputs = data.ManualInputs,
            InvalidInputs = data.InvalidInputs,
            HasInvalidOutput = data.HasInvalidOutput
        };
    }

    public override void LoadData(EditorNodeSaveData data)
    {
        base.LoadData(data);
    }
}