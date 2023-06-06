using Godot;
using System;

public partial class ExecInput : Node2D
{

    public delegate void SelectedEventHandler(ExecInput sender);
    /// <summary>
    /// Called when player selects this connector via ui
    /// </summary>
    public event SelectedEventHandler? Selected;

    [Export]
    public bool IsInput { get; set; } = false;

    public ExecInput? Connection { get; set; } = null;
    public EditorVisNode? OwningNode { get; set; } = null;

    public bool IsExecConnected => Connection != null;

    private void ConnectorSelected()
    {
        // because it would be very annoying to deal with node connections from inside the node itself
        // we instead just send an event to who ever listens and hope it will start/finish connections
        Selected?.Invoke(this);
    }

    public bool CanConnect(ExecInput other)
    {
        return !(Connection != null || other.OwningNode == OwningNode || other.IsInput == IsInput);
    }
}
