using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using VisLang.Editor.Parsing;

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

    /// <summary>
    /// Updates visual connection for a node by changing where left side of the line is connected
    /// </summary>
    /// <param name="source">Node that connection is coming from(will be unaffected)</param>
    /// <param name="portId">Port on the node that connection is coming from</param>
    /// <param name="dest">Destination node that needs connection updated</param>
    /// <param name="oldPortId">Which port it was</param>
    /// <param name="newPortId">Which port it should become</param>
    public void ChangePortsForInputConnection(EditorGraphNode source, int portId, EditorGraphNode dest, int oldPortId, int newPortId)
    {
        DisconnectNode(source.Name, portId, dest.Name, oldPortId);
        ConnectNode(source.Name, portId, dest.Name, newPortId);
    }

    /// <summary>
    /// Updates visual connection for a node by changing where right side of the line is connected
    /// </summary>
    /// <param name="source">Node that connection is coming from and needs to be updated/param>
    /// <param name="oldPortId">Which port it was</param>
    /// <param name="newPortId">Which port it should be</param>
    /// <param name="dest">Left side node that line is connected to</param>
    /// <param name="portId">Port on the destination node that will be unchanged</param>
    public void ChangePortsForOutputConnection(EditorGraphNode source, int oldPortId, int newPortId, EditorGraphNode dest, int portId)
    {
        DisconnectNode(source.Name, oldPortId, dest.Name, portId);
        ConnectNode(source.Name, newPortId, dest.Name, portId);
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
        node.ParentCanvas = this;
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
        IEnumerable<ConnectionInfo> incoming = GetNodeConnections().Where(p => p.Destination == node);
        IEnumerable<ConnectionInfo> outgoing = GetNodeConnections().Where(p => p.Source == node);
        // first clear incoming because i chose to do so first
        // works differently because source node does not care what connections it has on the output end
        // only exec cares 
        foreach (ConnectionInfo con in incoming)
        {
            // node here is Destination
            // exec port is on line 0
            if (con.Destination.IsExecutable && con.DestinationPortId == 0)
            {
                con.Source.NextExecutable = null;
            }
            DisconnectNode(con.Source.Name, con.SourcePortId, con.Destination.Name, con.DestinationPortId);
        }
        foreach (ConnectionInfo con in outgoing)
        {
            // node here is Source
            // exec port is on line 0
            if (con.Source.IsExecutable && con.DestinationPortId == 0)
            {
                con.Destination.PreviousExecutable = null;
            }
            else
            {
                con.Destination.DestroyConnectionOnPort(con.DestinationPortId, true);
            }
            DisconnectNode(con.Source.Name, con.SourcePortId, con.Destination.Name, con.DestinationPortId);
        }
        node.QueueFree();
    }

    /// <summary>
    /// Get data from GetConnectionList() as a collection of records<para></para>
    /// Removes the necessity of dealing with godot's python like Godot.Collections.Dictionary
    /// </summary>
    /// <returns></returns>
    public List<Parsing.ConnectionInfo> GetNodeConnections()
    {
        return GetConnectionList().Select(conn => new Parsing.ConnectionInfo
            (
                GetNode<EditorGraphNode>(conn["from"].AsString()),
                conn["from_port"].AsInt32(),
                GetNode<EditorGraphNode>(conn["to"].AsString()),
                conn["to_port"].AsInt32()
            )).ToList();
    }

    /// <summary>
    /// Iterates over all the children 
    /// </summary>
    private List<VisLang.DataNode> GenerateDataTreeForNode(EditorGraphNode node, VisSystem system)
    {
        List<VisLang.DataNode> inputs = new();
        foreach (EditorGraphNodeInput? input in node.Inputs)
        {
            if (input == null)
            {
                // TODO: this should place a const node here, but as nodes don't yet have this feature we just ignore and move on
                continue;
            }
            if (input.Node.IsExecutable)
            {
                throw new NotImplementedException("Support for passing result of exec node to exec node is not added yet");
            }
            // for data nodes we have to implement tree parsing that goes from left to right
            VisLang.DataNode? data = input.Node.CreateInterpretableNode<VisLang.DataNode>();
            if (data == null)
            {
                continue;
            }
            data.Interpreter = system;
            // recursively parse the data tree
            data.Inputs = GenerateDataTreeForNode(input.Node, system);
            inputs.Add(data);
        }
        return inputs;
    }

    /// <summary>
    /// Goes through all the nodes created by the user and generates VisLang nodes based on that
    /// </summary>
    /// <returns>A single root node</returns>
    public VisLang.ExecutionNode? GenerateNodeTree(VisSystem system)
    {
        List<Parsing.ConnectionInfo> connections = GetNodeConnections();
        // in future there could be an option of finding exec on the go
        // but this seems unnecessary at least for now
        if (ExecStart == null)
        {
            return null;
        }
        List<VisLang.VisNode?> nodes = new();
        EditorGraphNode? next = ExecStart.NextExecutable;
        VisLang.ExecutionNode? root = null;
        while (next != null)
        {
            VisLang.ExecutionNode? node = next.CreateInterpretableNode<VisLang.ExecutionNode>();
            if (node == null)
            {
                // nothing was created, logic can not continue
                return root;
            }
            node.Interpreter = system;
            root ??= node;
            node.DebugData = next;
            node.Inputs = GenerateDataTreeForNode(next, system);
            nodes.Add(node);

            next = next.NextExecutable;
        }
        GD.Print("Generated");
        return root;
    }
}
