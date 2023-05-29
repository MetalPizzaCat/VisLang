using Godot;
using System;

public partial class ConnectorLine : Node2D
{
    [Export]
    public Line2D Line { get; set; }

    public Vector2 Start
    {
        get => Line.Points[0];
        set
        {
            Line.SetPointPosition(0, value);
        }
    }

    public Vector2 End
    {
        get => Line.Points[Line.GetPointCount() - 1];
        set
        {
            Line.SetPointPosition(Line.GetPointCount() - 1, value);
        }
    }
}
