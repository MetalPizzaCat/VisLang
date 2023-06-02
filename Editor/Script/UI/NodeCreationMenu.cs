using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class NodeCreationMenu : PopupPanel
{
	public delegate void FunctionSelectedEventHandler(FunctionInfo info);
	public event FunctionSelectedEventHandler? FunctionSelected;


    [Export]
    public FunctionSignatureManager? Functions { get; set; }

    [Export]
    public VBoxContainer ItemContainer { get; set; }
    [Export]
    public PackedScene? ButtonPrefab { get; set; }

    public List<NodeCreationButton> Buttons { get; set; } = new();

    public override void _Ready()
    {
        base._Ready();
        if (Functions == null)
        {
            return;
        }
        foreach (FunctionInfo function in Functions.Functions)
        {
            NodeCreationButton? btn = ButtonPrefab?.InstantiateOrNull<NodeCreationButton>();
            if (btn == null)
            {
                GD.PrintErr("Unable to create button for node creation");
                return;
            }
			btn.Selected += NodeSelected;
            btn.Info = function;
            Buttons.Add(btn);
            ItemContainer.AddChild(btn);
        }
    }

    private void SearchTextChanged(string text)
    {
        Buttons.ForEach(p => p.Visible = p.Info?.FunctionName.StartsWith(text) ?? false);
    }

	private void NodeSelected(NodeCreationButton? button, FunctionInfo info)
	{
		FunctionSelected?.Invoke(info);
	}
}
