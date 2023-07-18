namespace VisLang.Editor;
using Godot;
using System;

public partial class ExecStartGraphNode : EditorGraphNode
{
    [Export]
    public CodeColorTheme? CodeTheme { get; set; }


    public override void _Ready()
    {
        if (Theme != null)
        {
            SetSlotTypeRight(0, EditorGraphNode.ExecTypeId);
        }
        CanBeDeleted = false;
        ShowClose = false;
    }
}
