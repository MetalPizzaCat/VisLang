using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using VisLang.Editor.Parsing;
using VisLang.Editor.Files;

namespace VisLang.Editor;

/// <summary>
/// Canvas on which nodes are moved, connected, created and removed
/// </summary>
public partial class NodeEditCanvas : GraphEdit
{
    protected record struct ProcessedNodeInfo(EditorGraphNode Node, VisLang.ExecutionNode CompiledNode);

    public delegate void NodeDeletedEventHandler(EditorGraphNode node);
    public event NodeDeletedEventHandler? NodeDeleted;

    public delegate void DeclarationSelectedEventHandler(EditorGraphFunctionDeclarationNode node);
    public event DeclarationSelectedEventHandler? DeclarationSelected;

    public delegate void DeclarationDeselectedEventHandler(EditorGraphFunctionDeclarationNode node);
    public event DeclarationDeselectedEventHandler? DeclarationDeselected;

    [Export]
    public NodeCreationMenu CreationMenu { get; private set; }

    [Export]
    public CodeColorTheme CodeTheme { get; set; }

    [Export]
    public ExecStartGraphNode? ExecStart { get; private set; }

    public List<EditorGraphNode> SelectedNodes { get; private set; } = new();

    /// <summary>
    /// All of the nodes that have breakpoint flag set
    /// </summary>
    public IEnumerable<EditorGraphNode> BreakpointNodes => GetChildren().OfType<EditorGraphNode>().Where(p => p.HasBreakpointEnabled);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // every type can only be connected to itself or "any" type
        foreach (VisLang.ValueType val in Enum.GetValues(typeof(VisLang.ValueType)))
        {
            AddValidConnectionType(EditorGraphNode.GetTypeIdForValueType(val), EditorGraphNode.AnyTypeId);
            AddValidConnectionType(EditorGraphNode.AnyTypeId, EditorGraphNode.GetTypeIdForValueType(val));
        }
        CreationMenu.FunctionSelected += SpawnFunction;
        CreationMenu.ConditionalNodeSelected += SpawnConditionalNode;
        CreationMenu.NewDeclarationNodeSelected += SpawnFunctionDeclarationNode;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (Input.IsActionJustPressed("breakpoint"))
        {
            ToggleBreakpoint();
        }
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

    /// <summary>
    /// Connects two nodes by their names
    /// </summary>
    /// <param name="sourceNode">Name of the node that connection is coming from(has port on the right)</param>
    /// <param name="sourcePort">Port on the source node</param>
    /// <param name="destNode">Name of the node that connection is going to(has port on the left)</param>
    /// <param name="destPort">Port on the destination node</param>
    private void ConnectNodes(string sourceNode, int sourcePort, string destNode, int destPort)
    {
        if (GetNodeOrNull<EditorGraphNode>(sourceNode) is EditorGraphNode source && GetNodeOrNull<EditorGraphNode>(destNode) is EditorGraphNode destination)
        {
            if (source.GetSlotTypeRight(sourcePort) == EditorGraphNode.ExecTypeId)
            {
                if (!source.CanConnectOnPortExec(sourcePort))
                {
                    return;
                }
                ConnectNode(sourceNode, sourcePort, destNode, destPort);
                source.AddExecConnection(sourcePort, destination, destPort);
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
        node.PositionOffset = GetGlobalMousePosition() - new Vector2(50, 50);
        node.GenerateFunction(info);
        node.DeleteRequested += DeleteNode;
        return node;
    }

    private void SpawnConditionalNode()
    {
        MakeNodeFromSignature<EditorGraphBranchNode>(null);
    }

    private void SpawnFunctionDeclarationNode()
    {
        EditorGraphFunctionDeclarationNode? node = MakeNodeFromSignature<EditorGraphFunctionDeclarationNode>(new FunctionInfo()
        {
            FunctionName = "Custom func",
            Inputs = new()
            {
                new FunctionInputInfo("test1", VisLang.ValueType.Float, null),
                new FunctionInputInfo("test2", VisLang.ValueType.Float, null)
            }
        });
        if (node != null)
        {
            node.EditorNodeSelected +=  (EditorGraphNode? edit) => DeclarationSelected?.Invoke(node);
            node.EditorNodeDeselected +=  (EditorGraphNode? edit) => DeclarationDeselected?.Invoke(node);
        }
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

    /// <summary>
    /// Deletes a node and removes all of the connections related to this node from the connected nodes<para/>
    /// This is the safer way to remove a node from the system as this way other nodes will not keep track of ghost nodes 
    /// </summary>
    /// <param name="node">node to delete</param>
    private void DeleteNode(EditorGraphNode? node)
    {
        if (node == null)
        {
            GD.PrintErr("Attempted to remove node but node was null, so there was nothing to remove");
            return;
        }
        NodeDeleted?.Invoke(node);
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
            con.Source.DestroyConnectionOnPort(con.SourcePortId, false);
            DisconnectNode(con.Source.Name, con.SourcePortId, con.Destination.Name, con.DestinationPortId);
        }
        foreach (ConnectionInfo con in outgoing)
        {
            // node here is Source
            // exec port is on line 0
            if (con.Source.IsExecutable && con.DestinationPortId == 0)
            {
                // con.Destination.PreviousExecutable = null;
                // we used to destroy reference to on the next node, but since exec lines are a one directional list now
                // there is nothing to destroy
                // TODO: Check if there is anything else that needs to be done here
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

    public void DeleteCollectionOfNodes(Godot.Collections.Array<string> nodes)
    {
        foreach (string nodeName in nodes)
        {
            EditorGraphNode? node = GetNodeOrNull<EditorGraphNode>(nodeName);
            if (node != null)
            {
                DeleteNode(node);
            }
        }
    }

    public void SelectNode(Node node)
    {
        if (node is EditorGraphNode graphNode)
        {
            SelectedNodes.Add(graphNode);
        }
    }

    public void DeselectNode(Node node)
    {
        if (node is EditorGraphNode graphNode)
        {
            SelectedNodes.Remove(graphNode);
        }
    }

    /// <summary>
    /// Toggles breakpoint for every node in the selected node array
    /// </summary>
    public void ToggleBreakpoint()
    {
        foreach (EditorGraphNode node in SelectedNodes)
        {
            node.HasBreakpointEnabled = !node.HasBreakpointEnabled;
        }
    }

    // These functions are meant for parsing the tree created by player into usable objects 
    #region Compilation
    /// <summary>
    /// Iterates over all the children 
    /// </summary>
    private List<VisLang.DataNode> GenerateDataTreeForNode(EditorGraphNode node, VisSystem system)
    {
        List<VisLang.DataNode> inputs = new();
        for (int i = 0; i < node.Inputs.Count; i++)
        {
            EditorGraphNodeInput? input = node.Inputs.ElementAtOrDefault(i);
            // if no input then we have to assume that there is a const value that can be read
            // technically some types don't have this but passing null to constructor will work well enough
            // because value type has checks in place for that, if not then oops :3
            if (input == null)
            {
                VisLang.VariableGetConstNode con = new()
                {
                    Value = new VisLang.Value(new VisLang.ValueTypeData(node.InputControls[i].InputType), node.InputControls[i].InputData)
                };
                inputs.Add(con);
                continue;
            }
            // TODO: Implement using result of exec
            // this should not be hard but it requires to know how many objects actually do refer to this result
            // as one solution, a result can be stored in the internal variable and any object could refer to it
            // as other count how many references to this input are present and call peek/pop enough times
            // first solution avoids possible issues when input is used down the line
            // can choose between solutions based on how many connections exec has
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

    private VisLang.ExecutionNode? GenerateExecNode(EditorGraphNode sourceNode, VisSystem system)
    {
        VisLang.ExecutionNode? node = sourceNode.CreateInterpretableNode<VisLang.ExecutionNode>();
        if (node == null)
        {
            // nothing was created, logic can not continue
            return null;
        }
        node.Interpreter = system;
        node.DebugData = sourceNode;
        node.Inputs = GenerateDataTreeForNode(sourceNode, system);
        return node;
    }

    private VisLang.ExecutionNode? ProcessLineOfExecutables
    (
        ref List<EditorGraphBranchNode> branches, // these might not need to be passed as references but i'll still mark them as such to make it clear that these WILL be edited
        ref List<ProcessedNodeInfo> nodes,
        EditorGraphNode? start,
        VisSystem system
    )
    {
        EditorGraphNode? next = start;
        VisLang.ExecutionNode? prev = null;
        VisLang.ExecutionNode? root = null;
        while (next != null)
        {
            // we hit a node that we processed already
            // but we still want to continue towards this node
            if (nodes.Any(p => p.Node == next))
            {
                if (prev != null)
                {
                    prev.DefaultNext = nodes.First(p => p.Node == next).CompiledNode;
                    break;
                }
                // there can be a situation where we start the loop and immediately hit an existing node
                // in this case no point in running the loop cause it would just be duplication
                return nodes.First(p => p.Node == next).CompiledNode;

            }
            EditorGraphNode? nodeToSwitchTo = null;
            if (next is EditorGraphBranchNode branch)
            {
                branches.Add(branch);
                nodeToSwitchTo = branch.SuccessNextExecutable?.Node;
            }
            else
            {
                nodeToSwitchTo = next.NextExecutable?.Node;
            }
            VisLang.ExecutionNode? node = GenerateExecNode(next, system);
            if (node == null)
            {
                // nothing was created, logic can not continue
                return root;
            }
            root ??= node;
            nodes.Add(new ProcessedNodeInfo(next, node));

            // if we have already created something that means it should know
            // about what we created right now 
            if (prev != null)
            {
                prev.DefaultNext = node;
            }
            prev = node;
            next = nodeToSwitchTo;
        }
        return root;
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
        List<ProcessedNodeInfo> nodes = new();
        List<EditorGraphBranchNode> branches = new();
        VisLang.ExecutionNode? root = ProcessLineOfExecutables(ref branches, ref nodes, ExecStart.NextExecutable?.Node, system);

        while (branches.Count > 0)
        {
            EditorGraphBranchNode? start = branches.FirstOrDefault();
            if (start == null)
            {
                break;
            }
            if (nodes.FirstOrDefault(p => p.Node == start).CompiledNode is VisLang.FlowControlIfNode branch)
            {
                VisLang.ExecutionNode? failLine = ProcessLineOfExecutables(ref branches, ref nodes, start.FailureNextExecutable?.Node, system);
                branch.FailureNext = failLine;
            }
            branches.Remove(start);
        }
        GD.Print("Generated");
        return root;
    }
    #endregion

    #region SaveSystem
    public NodeCollectionSaveData GenerateSaveData()
    {
        NodeCollectionSaveData data = new NodeCollectionSaveData() { ExecStartData = ExecStart?.GetSaveData() };
        data.ScrollOffset = ScrollOffset;
        foreach (EditorGraphNode node in GetChildren().Where(p => p is EditorGraphNode && p is not ExecStartGraphNode))
        {
            if (node is EditorGraphVariableNode variableNode)
            {
                data.VariableNodes.Add(variableNode.GetVariableSaveData());
            }
            if (node is EditorGraphBranchNode branch)
            {
                data.BranchNodes.Add(node.GetSaveData());
            }
            else
            {
                data.GenericNodes.Add(node.GetSaveData());
            }
        }

        data.Connections = GetNodeConnections().Select(p => new NodeCollectionSaveData.StoredConnectionInfo
        (
            p.Source.Name,
            p.SourcePortId,
            p.Destination.Name,
            p.DestinationPortId
        )).ToList();

        return data;
    }

    public void ClearCanvas()
    {
        IEnumerable<Node> kids = GetChildren().Where(p => p != ExecStart && p != CreationMenu && p is not FunctionSignatureManager);
        foreach (Node kid in kids)
        {
            kid.QueueFree();
            RemoveChild(kid);
        }
        ClearConnections();
        ExecStart.NextExecutable = null;
    }

    public void LoadSaveData(NodeCollectionSaveData data)
    {
        ScrollOffset = data.ScrollOffset;
        if (ExecStart != null && data.ExecStartData != null)
        {
            ExecStart.CanvasPosition = data.ExecStartData.Position;
        }
        foreach (EditorNodeSaveData node in data.GenericNodes)
        {
            EditorGraphNode? editorNode = null;
            if (!string.IsNullOrWhiteSpace(node.FunctionInfoResourcePath))
            {
                FunctionInfo info = ResourceLoader.Load<FunctionInfo>(node.FunctionInfoResourcePath);
                editorNode = MakeNodeFromSignature<EditorGraphNode>(info);
                if (editorNode == null)
                {
                    GD.PrintErr("Failed to create node from signature");
                    continue;
                }
            }
            editorNode?.LoadData(node);
        }
        foreach (EditorNodeSaveData node in data.BranchNodes)
        {
            MakeNodeFromSignature<EditorGraphBranchNode>(null)?.LoadData(node);
        }

        foreach (NodeCollectionSaveData.StoredConnectionInfo conn in data.Connections)
        {
            // because of how godots names nodes a node without a name is "@@" and since 
            // all editor nodes have no name it adds a number to the end
            // but we load this number becomes the name of the node
            // so we have to cut it out
            // technically i could cut it out when saving but there is no real difference
            ConnectNodes(conn.Source.TrimPrefix("@@"), conn.SourcePort, conn.Destination.TrimPrefix("@@"), conn.DestinationPort);
        }
    }

    #endregion
}
