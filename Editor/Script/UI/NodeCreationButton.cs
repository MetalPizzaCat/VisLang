using Godot;
using System;

public partial class NodeCreationButton : Button
{
    public delegate void SelectedEventHandler(NodeCreationButton? button, FunctionInfo function);
    public event SelectedEventHandler? Selected;

    [Export]
    public Label? FunctionNameLabel { get; set; }

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

    public override void _Ready()
    {
        base._Ready();
        Pressed += () => { if (Info != null) Selected?.Invoke(this, Info); };
    }
}
