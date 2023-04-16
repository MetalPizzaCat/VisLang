using System.Threading.Tasks;

namespace VisLang;
public class ExecutionNode : VisNode
{
    public ExecutionNode(VisSystem? interpreter) : base(interpreter)
    {
    }


    public virtual ExecutionNode? GetNext() => null;

    public List<DataNode> Inputs { get; set; } = new();

    public List<DataNode> Outputs { get; set; } = new();

    public virtual void Execute()
    {

    }
}
