using Godot;
using System;

/// <summary>
/// Button for creating nodes that use FunctionInfo for generation
/// </summary>
public partial class NodeCreationButton : NodeCreationButtonBase
{
    public delegate void SelectedEventHandler(NodeCreationButton? button, FunctionInfo function);
    public event SelectedEventHandler? Selected;

    private FunctionInfo? _info;

    [Export]
    public FunctionInfo? Info
    {
        get => _info;
        set
        {
            _info = value;
            if (_info == null)
            {
                return;
            }
            TooltipText = _info.FunctionDescription;
            if (FunctionNameLabel != null)
            {
                FunctionNameLabel.Text = _info.FunctionName;
            }
        }
    }

    public override string? FunctionName => Info?.FunctionName;

    public override void _Pressed()
    {
        base._Pressed();
        if (Info != null)
        {
            Selected?.Invoke(this, Info);
        }
    }
}
