namespace VisLang.Editor;

using VisLang.Editor.Files;

using Godot;
using System;
using System.Linq;
using System.Collections.Generic;


public partial class FunctionEditControl : Control
{
    [Export]
    public VariableManager VariableManager { get; private set; }

    [Export]
    public NodeEditCanvas NodeCanvas { get; private set; }

    private List<EditorGraphVariableNode> _variableNodes = new();

    public override void _Ready()
    {
        VariableManager.GetterRequested += SpawnGetter;
        VariableManager.SetterRequested += SpawnSetter;
        VariableManager.VariableNameChanged += UpdateVariableNodeNames;
        VariableManager.VariableTypeChanged += (VisLang.Editor.VariableInfo info, VisLang.ValueTypeData type) => { UpdateVariableNodes(info); };
        NodeCanvas.NodeDeleted += RemoveVariableNode;
    }

    private void RemoveVariableNode(EditorGraphNode node)
    {
        if (node is EditorGraphVariableNode varNode)
        {
            _variableNodes.Remove(varNode);
        }
    }

    public void UpdateVariableNodes(VisLang.Editor.VariableInfo info)
    {
        foreach (EditorGraphVariableNode node in _variableNodes.Where(p => p.Variable == info))
        {
            node.Variable = info;
        }
    }

    public void UpdateVariableNodeNames(VisLang.Editor.VariableInfo info, string name)
    {
        foreach (EditorGraphVariableNode node in _variableNodes.Where(p => p.Variable == info))
        {
            node.VariableName = name;
        }
    }

    private EditorGraphVariableNode? CreateGetSetNode(VariableInfo info, bool isGetter)
    {
        EditorGraphVariableNode? node = NodeCanvas.MakeNodeFromSignature<EditorGraphVariableNode>(new FunctionInfo());
        if (node != null)
        {
            node.IsGetter = isGetter;
            node.Variable = info;
            _variableNodes.Add(node);
        }
        return node;
    }

    public void SpawnGetter(VariableInfo info)
    {
        CreateGetSetNode(info, true);
    }

    public void SpawnSetter(VariableInfo info)
    {
        CreateGetSetNode(info, false);
    }

    public FunctionSaveData GetSaveData()
    {
        FunctionSaveData data = new();
        data.Nodes = NodeCanvas.GenerateSaveData();
        data.Variables = VariableManager.GetVariableInits();
        return data;
    }

    public void LoadSaveData(FunctionSaveData saveData)
    {
        List<VariableInfo> variables = new();
        foreach (VariableInitInfo info in saveData.Variables)
        {
            if (VariableManager.CreateVariableFromInfo(info) is VariableInfo variable)
            {
                variables.Add(variable);
            }
        }

        foreach (VariableNodeSaveData node in saveData.Nodes.VariableNodes)
        {
            VariableInfo? variable = variables.FirstOrDefault(p => p.Id == node.VariableId);
            if (variable == null)
            {
                continue;
            }
            EditorGraphVariableNode? getSet = CreateGetSetNode(variable, node.IsGetter);
            if (getSet != null)
            {
                getSet.LoadData(node);
            }
        }

        NodeCanvas.LoadSaveData(saveData.Nodes);
    }

    public void ClearCanvas()
    {
        _variableNodes.Clear();
        VariableManager.Clear();
        NodeCanvas.ClearCanvas();
    }
}
