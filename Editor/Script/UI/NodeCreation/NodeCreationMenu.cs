using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class NodeCreationMenu : PopupPanel
{
    public delegate void FunctionSelectedEventHandler(FunctionInfo info);
    public event FunctionSelectedEventHandler? FunctionSelected;
    public delegate void SpecialFunctionSelectedEventHandler(SpecialFunctionInfo info);
    public event SpecialFunctionSelectedEventHandler? SpecialFunctionSelected;

    [Export]
    public FunctionSignatureManager? Functions { get; set; }

    [Export]
    public VBoxContainer ItemContainer { get; set; }
    [Export]
    public PackedScene? ButtonPrefab { get; set; }
    [Export]
    public PackedScene? SpecialButtonPrefab { get; set; }

    public List<NodeCreationButtonBase> Buttons { get; set; } = new();

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
        foreach (SpecialFunctionInfo function in Functions.SpecialFunctions)
        {
            SpecialNodeCreationButton? btn = SpecialButtonPrefab?.InstantiateOrNull<SpecialNodeCreationButton>();
            if (btn == null)
            {
                GD.PrintErr("Unable to create button for node creation");
                return;
            }
            btn.Selected += SpecialNodeSelected;
            btn.Info = function;
            Buttons.Add(btn);
            ItemContainer.AddChild(btn);
        }
    }

    private void SearchTextChanged(string text)
    {
        Buttons.ForEach(p => p.Visible = p.FunctionName?.StartsWith(text, true, null) ?? false);
    }

    private void NodeSelected(NodeCreationButton? button, FunctionInfo info)
    {
        FunctionSelected?.Invoke(info);
    }

    private void SpecialNodeSelected(SpecialNodeCreationButton? button, SpecialFunctionInfo info)
    {
        SpecialFunctionSelected?.Invoke(info);
    }
}
