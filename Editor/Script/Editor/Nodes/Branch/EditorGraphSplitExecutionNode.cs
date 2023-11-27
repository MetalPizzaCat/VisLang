namespace VisLang.Editor;

public partial class EditorGraphSplitExecutionNode : EditorGraphNode
{
    public EditorGraphNodeInput? UpperNextExecutable { get; set; }

    public EditorGraphNodeInput? LowerNextExecutable { get; set; }

}