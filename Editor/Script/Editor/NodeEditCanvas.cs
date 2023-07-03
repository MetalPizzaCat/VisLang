using Godot;
using System;

namespace VisLang.Editor;

public partial class NodeEditCanvas : GraphEdit
{
    [Export]
    public EditorGraphNode TestNode1 { get; private set; }

    [Export]
    public EditorGraphNode TestNode2 { get; private set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        foreach (VisLang.ValueType val in Enum.GetValues(typeof(VisLang.ValueType)))
        {
            AddValidConnectionType((int)val + 2, EditorGraphNode.AnyTypeId);
            AddValidConnectionType(EditorGraphNode.AnyTypeId, (int)val + 2);
        }

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


        GD.Print($"sourceNode: {sourceNode}, sourcePort: {sourcePort}, destNode: {destNode}, destPort{destPort}");
    }
}
