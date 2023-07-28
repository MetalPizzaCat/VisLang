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

    public void SpawnGetter(VariableInfo info)
    {
        EditorGraphVariableNode? node = NodeCanvas.MakeNodeFromSignature<EditorGraphVariableNode>(new FunctionInfo());
        if (node != null)
        {
            node.IsGetter = true;
            node.Variable = info;
            _variableNodes.Add(node);
        }
    }

    public void SpawnSetter(VariableInfo info)
    {
        EditorGraphVariableNode? node = NodeCanvas.MakeNodeFromSignature<EditorGraphVariableNode>(new FunctionInfo());
        if (node != null)
        {
            node.IsGetter = false;
            node.Variable = info;
            _variableNodes.Add(node);
        }
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
        foreach (VariableInitInfo info in saveData.Variables)
        {
            VariableManager.CreateVariableFromInfo(info);
        }
    }

    public void ClearCanvas()
    {
        _variableNodes.Clear();
        VariableManager.Clear();
        NodeCanvas.ClearCanvas();
    }
}
