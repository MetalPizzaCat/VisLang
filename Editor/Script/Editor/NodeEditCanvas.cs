using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

namespace VisLang.Editor;

public partial class NodeEditCanvas : GraphEdit
{

    [Export]
    public NodeCreationMenu CreationMenu { get; private set; }

    [Export]
    public CodeColorTheme CodeTheme { get; set; }

    [Export]
    public ExecStartGraphNode? ExecStart { get; private set; }

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
        node.DeleteRequested += DeleteNode;
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
        // i love this check and cast feature of c# :3
        if (GetNodeOrNull<EditorGraphNode>(sourceNode) is EditorGraphNode source && GetNodeOrNull<EditorGraphNode>(destNode) is EditorGraphNode destination)
        {
            destination.DestroyConnectionOnPort(destPort, true);
            source.DestroyConnectionOnPort(sourcePort, false);
        }

        DisconnectNode(sourceNode, sourcePort, destNode, destPort);
        GD.Print($"Disconnect -> sourceNode: {sourceNode}, sourcePort: {sourcePort}, destNode: {destNode}, destPort{destPort}");
    }

    private void DeleteNode(EditorGraphNode? node)
    {
        if (node == null)
        {
            GD.PrintErr("Attempted to remove node but node was null, so there was nothing to remove");
            return;
        }
        // first clear all incoming connections and then outgoing, no real reason why this order i just felt like it
        // while we could do so in one loop we have to manually undo connections in nodes that we are disconnecting from
        // since these connections are only used for visual representation
        foreach (Godot.Collections.Dictionary dict in GetConnectionList().Where(dict => dict["to"].AsString() == node.Name))
        {
            int destPort = dict["to_port"].AsInt32();
            // exec ports should always be on line 0
            if (node.IsExecutable && destPort == 0)
            {
                if (GetNodeOrNull<EditorGraphNode>(dict["from"].AsString()) is EditorGraphNode source)
                {
                    source.NextExecutable = null;
                }
            }
            DisconnectNode(dict["from"].AsString(), dict["from_port"].AsInt32(), node.Name, destPort);
        }
        foreach (Godot.Collections.Dictionary dict in GetConnectionList().Where(dict => dict["from"].AsString() == node.Name))
        {
            int toPort = dict["to_port"].AsInt32();
            if (GetNodeOrNull<EditorGraphNode>(dict["from"].AsString()) is EditorGraphNode dest)
            {
                // exec ports should always be on line 0
                if (node.IsExecutable && toPort == 0)
                {

                    dest.PreviousExecutable = null;

                }
                else
                {
                    dest.Inputs[toPort] = null;
                }
            }
            DisconnectNode(node.Name, toPort, dict["to"].AsString(), dict["to_port"].AsInt32());
            node.QueueFree();
        }
    }

    /// <summary>
    /// Get data from GetConnectionList() as a collection of records<para></para>
    /// Removes the necessity of dealing with godot's python like Godot.Collections.Dictionary
    /// </summary>
    /// <returns></returns>
    private List<Parsing.ConnectionInfo> GetNodeConnections()
    {
        return GetConnectionList().Select(conn => new Parsing.ConnectionInfo
            (
                GetNode<EditorGraphNode>(conn["from"].AsString()),
                conn["from_port"].AsInt32(),
                GetNode<EditorGraphNode>(conn["to"].AsString()),
                conn["to_port"].AsInt32()
            )).ToList();
    }

    public void GenerateNodeTree()
    {
        List<Parsing.ConnectionInfo> connections = GetNodeConnections();
        // in future there could be an option of finding exec on the go
        // but this seems unnecessary at least for now
        if (ExecStart == null)
        {
            return;
        }
        List<VisLang.VisNode?> nodes = new();
        EditorGraphNode? next = ExecStart.NextExecutable;
        while (next != null)
        {
            nodes.Add(next.CreateExecutableNode<VisLang.ExecutionNode>());
            next = next.NextExecutable;
        }
        GD.Print("Generated");
    }
}
