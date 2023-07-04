using Godot;
using System;

namespace VisLang.Editor;

public partial class NodeEditCanvas : GraphEdit
{

    [Export]
    public NodeCreationMenu CreationMenu { get; private set; }

    [Export]
    public CodeColorTheme CodeTheme { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        foreach (VisLang.ValueType val in Enum.GetValues(typeof(VisLang.ValueType)))
        {
            AddValidConnectionType((int)val + 2, EditorGraphNode.AnyTypeId);
            AddValidConnectionType(EditorGraphNode.AnyTypeId, (int)val + 2);
        }
        CreationMenu.FunctionSelected += SpawnFunction;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    private void ConnectNodes(string sourceNode, int sourcePort, string destNode, int destPort)
    {
        if (GetNodeOrNull<EditorGraphNode>(sourceNode) is EditorGraphNode source && GetNodeOrNull<EditorGraphNode>(destNode) is EditorGraphNode destination)
        {
            if (source.GetSlotTypeRight(sourcePort) == EditorGraphNode.ExecTypeId)
            {
                if (!destination.CanConnectOnPortExec(destPort, false) || !source.CanConnectOnPortExec(destPort, true))
                {
                    return;
                }
                ConnectNode(sourceNode, sourcePort, destNode, destPort);
                source.NextExecutable = destination;
                destination.PreviousExecutable = source;
            }
            else
            {
                if (!destination.CanConnectOnPort(destPort))
                {
                    return;
                }
                ConnectNode(sourceNode, sourcePort, destNode, destPort);
                destination.AddConnection(destPort, source, sourcePort);
            }
        }
        else if (GetNodeOrNull<EditorGraphNode>(sourceNode) is EditorGraphNode execStart && GetNodeOrNull<EditorGraphNode>(destNode) is EditorGraphNode next)
        {

        }


        GD.Print($"Connect -> sourceNode: {sourceNode}, sourcePort: {sourcePort}, destNode: {destNode}, destPort{destPort}");
    }

    private void OpenCreationMenu(Vector2 position)
    {
        CreationMenu.Position = new Vector2I((int)position.X, (int)position.Y);
        CreationMenu.Popup();
    }


    /// <summary>
    /// Generation function node at runtime based of provided function signature of given node type
    /// </summary>
    /// <param name="info">Function signature used to generate inputs</param>
    /// <typeparam name="EditorNodeType">Type of the node</typeparam>
    /// <returns>Generated node or null if generation failed for any reason</returns>
    public EditorNodeType? MakeNodeFromSignature<EditorNodeType>(FunctionInfo info) where EditorNodeType : EditorGraphNode, new()
    {
        EditorNodeType? node = new EditorNodeType();
        if (node == null)
        {
            return null;
        }
        node.CodeTheme = CodeTheme;
        AddChild(node);
        node.Position = GetGlobalMousePosition();
        node.GenerateFunction(info);
        return node;
    }

    /// <summary>
    /// Generate function node at runtime based on provided function signature.<para></para> 
    /// This version only creates base node type
    /// </summary>
    /// <param name="info">Function signature to use for node generation</param>
    private void SpawnFunction(FunctionInfo info)
    {
        MakeNodeFromSignature<EditorGraphNode>(info);
    }

    private void DisconnectNodes(string sourceNode, int sourcePort, string destNode, int destPort)
    {
        GD.Print($"Disconnect -> sourceNode: {sourceNode}, sourcePort: {sourcePort}, destNode: {destNode}, destPort{destPort}");
    }
}
