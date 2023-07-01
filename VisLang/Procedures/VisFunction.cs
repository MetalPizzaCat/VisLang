namespace VisLang;

/// <summary>
/// Functions represent data node only collections of nodes, that can never execute and modify anything. 
/// </summary>
public class VisFunction : VisCallable
{
    public VisFunction() { }

    public VisFunction(VisSystem mainSystem) : base(mainSystem) { }
    public DataNode? Root { get; set; } = null;
}