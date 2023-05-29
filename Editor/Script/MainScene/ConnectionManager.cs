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

    private bool _isPerformingExecConnection = false;

    private List<ConnectorLine> _connections = new();

    protected bool IsPerformingExecConnection
    {
        get => _isPerformingExecConnection;
        set
        {
            _isPerformingExecConnection = value;
            ConnectionPreview.Visible = value;
        }
    }

    public void SelectExecConnector(ExecInput exec)
    {
        if (IsPerformingExecConnection && SelectedExecConnector != null)
        {
            // we count is cancellation because this seems intuitive
            // but is it?
            if (SelectedExecConnector == exec)
            {
                IsPerformingExecConnection = false;
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
            IsPerformingExecConnection = true;
            SelectedExecConnector = exec;
            ConnectionPreview.Start = exec.GlobalPosition;
        }
    }

    private void CreateExecConnection(ExecInput input, ExecInput output)
    {
        if (ConnectorPrefab == null || Canvas == null)
        {
            return;
        }
        ConnectorLine line = ConnectorPrefab.InstantiateOrNull<ConnectorLine>();

        line.Start = input.GlobalPosition;
        line.End = output.GlobalPosition;
        _connections.Add(line);
        Canvas.AddChild(line);

        IsPerformingExecConnection = false;
        SelectedExecConnector = null;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseMotion mouseMotion && _isPerformingExecConnection)
        {
            ConnectionPreview.End = MouseLocation - (CanvasControl?.DefaultViewportSize ?? Vector2.Zero) / 2f;
        }
    }
}