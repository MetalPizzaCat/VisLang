using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class NodeCreationMenu : PopupPanel
{
    public delegate void FunctionSelectedEventHandler(FunctionInfo info);
    public delegate void SpecialFunctionSelectedEventHandler(SpecialFunctionInfo info);
    public delegate void ConditionalNodeSelectedEventHandler();
    public event FunctionSelectedEventHandler? FunctionSelected;
    public event ConditionalNodeSelectedEventHandler? ConditionalNodeSelected;
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

    private Button _conditionalNodeSpawnerButton = new Button();

    public override void _Ready()
    {
        base._Ready();
        CreateConditionalSpawnButton();
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

    private void CreateConditionalSpawnButton()
    {
        _conditionalNodeSpawnerButton.Pressed += () => { ConditionalNodeSelected?.Invoke(); };
        _conditionalNodeSpawnerButton.Text = "If / Branch";
        _conditionalNodeSpawnerButton.Alignment = HorizontalAlignment.Left;
        ItemContainer.AddChild(_conditionalNodeSpawnerButton);
    }

    private void SearchTextChanged(string text)
    {
        Buttons.ForEach(p => p.Visible = p.FunctionName?.ToLower().StartsWith(text, true, null) ?? false);
        // unreal uses 'branch', code uses 'if'  and i want more :3
        _conditionalNodeSpawnerButton.Visible = "If".ToLower().StartsWith(text, true, null) || "Branch".ToLower().StartsWith(text, true, null) || "Conditional".ToLower().StartsWith(text, true, null);
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
