using System;
using Godot;
using System.Collections.Generic;
using System.Linq;


/// <summary>
/// Class that is responsible for handling all of the connection logic
/// </summary>
public partial class ConnectionManager : Node
{
    [Export]
    public PackedScene? ConnectorPrefab { get; set; }

    [Export]
    public ConnectorLine ConnectionPreview { get; set; }

    [Export]
    public CanvasControl? CanvasControl { get; set; }

    [Export]
    public Node2D? Canvas { get; set; }


    /// <summary>
    /// Current position of the mouse
    /// </summary>
    public Vector2 MouseLocation => CanvasControl?.MousePosition ?? Vector2.Zero;

    protected ExecInput? SelectedExecConnector { get; set; }

    protected NodeInput? SelectedInputConnector { get; set; }

    private bool _isPerformingConnection = false;

    private List<ConnectorLine> _connections = new();

    protected bool IsPerformingConnection
    {
        get => _isPerformingConnection;
        set
        {
            _isPerformingConnection = value;
            ConnectionPreview.Visible = value;
        }
    }

    public void SelectInputConnector(NodeInput input)
    {
        if (SelectedExecConnector != null)
        {
            // we simply ignore possibility of connecting data to exec
            return;
        }
        if (IsPerformingConnection && SelectedInputConnector != null)
        {
            if (SelectedInputConnector == input)
            {
                IsPerformingConnection = false;
                SelectedInputConnector = null;
                return;
            }
            if (SelectedInputConnector.IsInput == input.IsInput)
            {
                return;
            }
            CreateInputConnection(input.IsInput ? SelectedInputConnector : input, input.IsInput ? input : SelectedInputConnector);
        }
        else
        {
            // if we select input connector and it already has a connection we should grab existing one
            // instead of creating a new one, as this way we can implement disconnection 
            if (input.IsInput && input.IsInputConnected)
            {
                ConnectorLine? line = GetConnectorLine(input.Connection, input);
                if (line != null)
                {
                    // destroy existing line because we will just create a new one
                    // and remove the connection
                    // basically treat occupied input selection as connection destruction 
                    _connections.Remove(line);
                    line.QueueFree();
                    // however because it looks better when connection is reset from the source rather then destination
                    // we have to switch which node we will operate upon
                    NodeInput node = input.Connection;
                    input.Connection = null;
                    input = node;
                }
            }
            IsPerformingConnection = true;
            SelectedInputConnector = input;
            ConnectionPreview.Start = input.GlobalPosition;
        }
    }

    private ConnectorLine? GetConnectorLine(Node src, Node dst)
    {
        return _connections.FirstOrDefault(p => p.StartNode == src && p.EndNode == dst);
    }

    /// <summary>
    /// Creates connection object between two given nodes. Connection is only stored in the connection manager and is purely visual
    /// </summary>
    /// <param name="src">Start node</param>
    /// <param name="dst">End node</param>
    private void CreateConnection(Node2D src, Node2D dst)
    {
        ConnectorLine? line = ConnectorPrefab?.InstantiateOrNull<ConnectorLine>();
        if (line == null)
        {
            throw new NullReferenceException("Failed to create connection line");
        }

        line.Start = src.GlobalPosition;
        line.End = dst.GlobalPosition;
        line.StartNode = src;
        line.EndNode = dst;
        _connections.Add(line);
        Canvas?.AddChild(line);
        IsPerformingConnection = false;
        SelectedInputConnector = null;
        SelectedExecConnector = null;
    }

    /// <summary>
    /// Creates connection using CreateConnection function but also sets destination and source values for nodes
    /// </summary>
    /// <param name="source">Start node</param>
    /// <param name="destination">End node</param>
    private void CreateInputConnection(NodeInput source, NodeInput destination)
    {
        if (ConnectorPrefab == null || Canvas == null || !destination.CanConnect(source))
        {
            return;
        }
        CreateConnection(source, destination);
        // we only tell destination what it's connected to because source can have as many connections as it wants
        destination.Connection = source;
    }

    public void SelectExecConnector(ExecInput exec)
    {
        if (SelectedInputConnector != null)
        {
            // we simply ignore possibility of connecting data to exec
            return;
        }
        if (IsPerformingConnection && SelectedExecConnector != null)
        {
            // we count is cancellation because this seems intuitive
            // but is it?
            if (SelectedExecConnector == exec)
            {
                IsPerformingConnection = false;
                SelectedExecConnector = null;
                return;
            }
            // check if they are not the same otherwise you might be able to create confusion loops :c
            if (SelectedExecConnector.IsInput == exec.IsInput)
            {
                return;
            }
            // ternary looks dumb here but it does it's job well
            CreateExecConnection(SelectedExecConnector.IsInput ? SelectedExecConnector : exec, !SelectedExecConnector.IsInput ? SelectedExecConnector : exec);
        }
        else
        {
            // if we grabbed node with existing connection
            // we have to split the connection because unlike data connections
            // exec can only have one source and one destination
            if (exec.IsExecConnected)
            {
                // use null operator to just check both options
                // reinventing ternary with Lilith :3
                ConnectorLine? line = GetConnectorLine(exec, exec.Connection) ?? GetConnectorLine(exec.Connection, exec);
                if (line != null)
                {
                    line.QueueFree();
                    _connections.Remove(line);
                    // we have to do a little switch of what are we operating upon because 
                    // it looks better this way
                    ExecInput node = exec.Connection;
                    exec.Connection = null;
                    node.Connection = null;
                    exec = node;
                }

            }
            IsPerformingConnection = true;
            SelectedExecConnector = exec;
            ConnectionPreview.Start = exec.GlobalPosition;
        }
    }

    /// <summary>
    /// Creates connection using CreateConnection function but also sets destination and source values for nodes
    /// </summary>
    /// <param name="source">Start node</param>
    /// <param name="destination">End node</param>
    private void CreateExecConnection(ExecInput input, ExecInput output)
    {
        if (ConnectorPrefab == null || Canvas == null || !input.CanConnect(output))
        {
            return;
        }
        CreateConnection(input, output);
        // because this is exec connection both output and input can only be connected to one other node
        output.Connection = input;
        input.Connection = output;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseMotion mouseMotion && _isPerformingConnection)
        {
            ConnectionPreview.End = MouseLocation - (CanvasControl?.DefaultViewportSize ?? Vector2.Zero) / 2f;
        }
    }
}