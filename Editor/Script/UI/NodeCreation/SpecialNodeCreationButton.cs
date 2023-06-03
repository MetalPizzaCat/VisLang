using System;
using Godot;

/// <summary>
/// Button for creating special nodes(nodes like Branch,Get,Set,For,etc.) that uses PackedScene for nodes
/// </summary>
public partial class SpecialNodeCreationButton : NodeCreationButtonBase
{
    public delegate void SelectedEventHandler(SpecialNodeCreationButton? sender, SpecialFunctionInfo node);
    public event SelectedEventHandler? Selected;
    private SpecialFunctionInfo? _info;

    [Export]
    public SpecialFunctionInfo? Info
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

    public override void _Pressed()
    {
        base._Pressed();
        if (Info != null)
        {
            Selected?.Invoke(this, Info);
        }
    }
}