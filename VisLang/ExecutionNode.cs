using System.Threading.Tasks;

namespace VisLang;
public class ExecutionNode : VisNode
{
    public ExecutionNode(VisSystem? interpreter) : base(interpreter)
    {
    }

    public ExecutionNode()
    {
    }

    public ExecutionNode? DefaultNext { get; set; } = null;

    public virtual ExecutionNode? GetNext() => DefaultNext;

    public virtual void Execute()
    {

    }
}
