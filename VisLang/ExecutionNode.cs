using System.Threading.Tasks;

namespace VisLang;
public class ExecutionNode : VisNode
{
    public ExecutionNode(VisSystem? interpreter) : base(interpreter)
    {
    }


    public ExecutionNode? Next { get; set; } = null;

    public List<DataNode> Inputs { get; set; } = new();

    public virtual void Execute()
    {

    }
}
