using System.Threading.Tasks;

namespace VisLang;
public class ExecutionNode : VisNode
{
    public ExecutionNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public ExecutionNode? DefaultNext { get; set; } = null;

    public virtual ExecutionNode? GetNext() => DefaultNext;
    
    public List<DataNode> Outputs { get; set; } = new();

    public virtual void Execute()
    {

    }
}
