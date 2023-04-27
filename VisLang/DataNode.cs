namespace VisLang;

/// <summary>
/// Data node is a type of node that can not be executed by can be used to generate or get data<para/>
/// For example DataNode can be used to implement Variable value getting node or arithmetic operations
/// </summary>
public class DataNode : VisNode
{
    public DataNode(VisSystem? interpreter) : base(interpreter)
    {
    }
}
