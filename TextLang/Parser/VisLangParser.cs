namespace TextLang.Parser;
using Sprache;
using VisLang;

public class TypeInfo
{
    public TypeInfo(VisLang.ValueType type, VisLang.ValueType? arrayDataType)
    {
        Type = type;
        ArrayDataType = arrayDataType;
    }

    public VisLang.ValueType Type { get; set; }
    public VisLang.ValueType? ArrayDataType { get; set; }
}
public class VariableInitInfo
{
    public VariableInitInfo(string name, TypeInfo type)
    {
        Name = name;
        Type = type;
    }

    public string Name { get; set; }

    public TypeInfo Type { get; set; }

}

public class ParsedProgramInfo
{
    public ParsedProgramInfo(IEnumerable<VariableInitInfo> variables, IEnumerable<ExecutionNode> nodes)
    {
        Variables = variables;
        Nodes = nodes;
    }

    public IEnumerable<VariableInitInfo> Variables { get; set; }

    public IEnumerable<VisLang.ExecutionNode> Nodes { get; set; }
}
public static class VisLangParser
{
    private static Parser<string> _quotedText = (
        from open in Parse.Char('\'')
        from content in Parse.CharExcept('\'').Many().Text()
        from close in Parse.Char('\'')
        select content
    );
    private static Parser<TypeInfo> _typeParser =
       (from type in (Parse.String("char").Token().Return(VisLang.ValueType.Char)
            .Or(Parse.String("int").Token().Return(VisLang.ValueType.Integer))
            .Or(Parse.String("float").Token().Return(VisLang.ValueType.Float))
            .Or(Parse.String("bool").Token().Return(VisLang.ValueType.Bool))
            .Or(Parse.String("string").Token().Return(VisLang.ValueType.String))
            .Or(Parse.String("ptr").Token().Return(VisLang.ValueType.Address)))
        from array in Parse.String("[]").Token().Optional()
        select array.IsEmpty ? new TypeInfo(type, null) : new TypeInfo(VisLang.ValueType.Array, type));

    private static Parser<VisLang.PrintNode> _printFunctionParser = (
        from keyword in Parse.String("print").Token()
        from lb in Parse.Char('(').Token()
        from content in _quotedText.Token().Select(p => new VisLang.VariableGetConstNode() { Value = new VisLang.Value(VisLang.ValueType.String, p) })
                            .Or<VisLang.DataNode>(Parse.CharExcept(')').AtLeastOnce().Text().Token().Select(p => new VisLang.VariableGetNode() { Name = p }))
        from rb in Parse.Char(')').Token()
        select new VisLang.PrintNode() { Inputs = new() { content } }
    );

    private static Parser<VariableInitInfo> _variableInitParser =
    (
        from keyword in Parse.String("let").Token()
        from name in Parse.Letter.AtLeastOnce().Text().Token()
        from colon in Parse.Char(':').Token()
        from type in _typeParser
        select new VariableInitInfo(name, type)
    );

    private static Parser<ParsedProgramInfo> _baseProgramParser =
    (
        from keyword in Parse.String("program").Token()
        from name in Parse.Letter.AtLeastOnce().Text().Token().Optional()
        from lb in Parse.Char('{').Token()
        from vars in _variableInitParser.Many()
        from functions in _printFunctionParser.Many()
        from rb in Parse.Char('}').Token()
        select new ParsedProgramInfo(vars, functions)
    );

    /// <summary>
    /// Recursively inits all of the  nodes connected to the src and source itself
    /// </summary>
    /// <param name="src">Node to start the proceeds from</param>
    private static void InitNode(VisLang.VisNode src, VisLang.VisSystem system)
    {
        src.Interpreter = system;
        foreach (VisLang.VisNode input in src.Inputs)
        {
            InitNode(input, system);
        }
    }

    public static VisLang.VisSystem? ParseCodeNoComments(string code)
    {
        ParsedProgramInfo program = _baseProgramParser.Parse(code);
        VisLang.VisSystem system = new VisLang.VisSystem();
        foreach (VariableInitInfo info in program.Variables)
        {
            system.VisSystemMemory.CreateVariable(info.Name, info.Type.Type, null, info.Type.ArrayDataType);
        }
        VisLang.ExecutionNode? root = null;
        VisLang.ExecutionNode? prev = null;
        foreach (VisLang.ExecutionNode node in program.Nodes)
        {
            if (root == null)
            {
                root = node;
            }
            if (prev != null)
            {
                prev.DefaultNext = node;
            }
            InitNode(node, system);

            prev = node;
        }
        system.Entrance = root;
        return system;
    }
}