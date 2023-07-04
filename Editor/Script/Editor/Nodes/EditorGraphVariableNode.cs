namespace VisLang.Editor;
using System.Collections.Generic;

public partial class EditorGraphVariableNode : EditorGraphNode
{
    private VariableInfo _variable;
    public bool IsGetter { get; set; } = true;
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
}