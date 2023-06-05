using Godot;
using System;

public partial class NodeMovementManager : Node
{
    [Export]
    public CanvasControl? CanvasControl { get; set; }

    [Export]
    public Vector2 GridCellSize { get; set; } = new Vector2(16, 16);

    public EditorVisNode? SelectedNode { get; set; } = null;

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseMotion && SelectedNode != null && CanvasControl != null)
        {
            SelectedNode.GlobalPosition = (CanvasControl.MousePosition - (CanvasControl?.DefaultViewportSize ?? Vector2.Zero) / 2f).Snapped(GridCellSize);
        }
    }
}
