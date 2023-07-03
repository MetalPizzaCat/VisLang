namespace VisLang.Editor;

public class EditorGraphNodeInput
{
    public EditorGraphNodeInput(EditorGraphNode node, int portId)
    {
        Node = node;
        PortId = portId;
    }

    public EditorGraphNode Node { get; set; }
    public int PortId { get; set; }
}