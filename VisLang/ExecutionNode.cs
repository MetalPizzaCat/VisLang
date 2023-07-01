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

    /// <summary>
    /// Execute given node. Execution can change global state(for example change a variable value)
    /// </summary>
    /// <param name="context">Additional context that can be used to tell node what to operate on</param>
    public virtual void Execute(NodeContext? context = null)
    {

    }
}
