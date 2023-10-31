using Godot;
using VisLang;

/// <summary>
/// Wrapper around ExecutionNode class so it would be possible to pass it as argument to fields that require Godot.Variant</para>
/// Annoying but it's a simple work around that doesn't require editing vislang interpreter itself
/// </summary>
public partial class ExecutionNodeData : GodotObject
{
    public ExecutionNode Node { get; }

    public ExecutionNodeData(ExecutionNode node)
    {
        Node = node;
    }
}