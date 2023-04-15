namespace VisLang;

public class VisNode
{
    public VisNode(VisSystem? interpreter)
    {
        Interpreter = interpreter;
    }

    public VisSystem? Interpreter { get; set; } = null;
}