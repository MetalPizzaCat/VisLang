using System;
using Godot;
using System.Collections.Generic;

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
            if (SelectedInputConnector.InputType != input.InputType)
            {
                return;
            }
            CreateInputConnection(input.IsInput ? SelectedInputConnector : input, input.IsInput ? input : SelectedInputConnector);
        }
        else
        {
            IsPerformingConnection = true;
            SelectedInputConnector = input;
            ConnectionPreview.Start = input.GlobalPosition;
        }
    }

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

    public void CreateInputConnection(NodeInput source, NodeInput destination)
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
            IsPerformingConnection = true;
            SelectedExecConnector = exec;
            ConnectionPreview.Start = exec.GlobalPosition;
        }
    }

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