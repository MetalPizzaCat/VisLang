namespace VisLang.Editor;
using Godot;
using System;

public partial class ExecStartGraphNode : GraphNode
{
    [Export]
    public CodeColorTheme? CodeTheme { get; set; }

	 /// <summary>
    /// Node that is connected to be executed next
    /// </summary>
    public EditorGraphNode? NextExecutable { get; set; } = null;
	
    public override void _Ready()
    {
        if (Theme != null)
        {
            SetSlotTypeRight(0, EditorGraphNode.ExecTypeId);
        }
    }
}
