namespace VisLang;

public class VisNode
{
    public VisNode()
    {
    }

    public VisNode(VisSystem? interpreter)
    {
        Interpreter = interpreter;
    }

    public List<VisNode> Inputs { get; set; } = new();

    public VisSystem? Interpreter { get; set; } = null;

    public virtual Value? GetValue()
    {
        return null;
    }
}