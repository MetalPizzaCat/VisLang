using Godot;
using System;

public partial class ConnectorLine : Node2D
{
    [Export]
    public Line2D Line { get; set; }

    private Node2D? _anchorStart;
    private Node2D? _anchorEnd;

    public Vector2 Start
    {
        get => _anchorStart?.GlobalPosition ?? Line.Points[0];
        set
        {
            Line.SetPointPosition(0, value);
        }
    }

    public Vector2 End
    {
        get => _anchorEnd?.GlobalPosition ?? Line.Points[Line.GetPointCount() - 1];
        set
        {
            Line.SetPointPosition(Line.GetPointCount() - 1, value);
        }
    }

    public Node2D? StartNode
    {
        get => _anchorStart;
        set
        {
            _anchorStart = value;
            if (value != null)
            {
                Start = value.GlobalPosition;
            }
        }
    }

    public Node2D? EndNode
    {
        get => _anchorEnd;
        set
        {
            _anchorEnd = value;
            if (value != null)
            {
                End = value.GlobalPosition;
            }
        }
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        // TODO: Possibly add check to make it run ONLY if connected nodes are moved, to avoid unnecessary updates
        if (@event is not InputEventMouseMotion)
        {
            return;
        }
        if (StartNode != null)
        {
            Start = StartNode.GlobalPosition;
        }
        if (EndNode != null)
        {
            End = EndNode.GlobalPosition;
        }
    }
}
