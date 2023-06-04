using Godot;
using System;

public partial class NodeCreationButtonBase : Button
{
    [Export]
    public Label? FunctionNameLabel { get; set; }

    public virtual string? FunctionName => null;

    public override void _Ready()
    {
        base._Ready();
    }
}
