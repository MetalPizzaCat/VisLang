namespace VisLang;

public class VisFunction : VisCallable
{
    public VisFunction() { }

    public VisFunction(VisSystem mainSystem) : base(mainSystem) { }
    public DataNode? Root { get; set; } = null;
}