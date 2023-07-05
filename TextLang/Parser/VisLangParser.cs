namespace TextLang.Parser;
using Sprache;
using VisLang;

public class VariableInitInfo
{
    public VariableInitInfo(string name, ValueTypeData type)
    {
        Name = name;
        Type = type;
    }

    public string Name { get; set; }

    public ValueTypeData Type { get; set; }

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
    private static readonly Parser<string> _variableName = Parse.Identifier(Parse.Letter.Or(Parse.Char('_')), Parse.LetterOrDigit.Or(Parse.Char('_'))).Text()
    .Except(
        // key words
        Parse.String("print")
        .Or(Parse.String("program"))
        .Or(Parse.String("let"))
    );
    private static readonly Parser<string> _quotedText = (
        from open in Parse.Char('\'')
        from content in Parse.CharExcept('\'').Many().Text()
        from close in Parse.Char('\'')
        select content
    ).Named("quoted text");

    private static readonly Parser<ValueTypeData> _typeParser =
       (from type in (Parse.String("char").Token().Return(VisLang.ValueType.Char)
            .Or(Parse.String("int").Token().Return(VisLang.ValueType.Integer))
            .Or(Parse.String("float").Token().Return(VisLang.ValueType.Float))
            .Or(Parse.String("bool").Token().Return(VisLang.ValueType.Bool))
            .Or(Parse.String("string").Token().Return(VisLang.ValueType.String))
            .Or(Parse.String("ptr").Token().Return(VisLang.ValueType.Address)))
        from array in Parse.String("[]").Token().Optional()
        select array.IsEmpty ? new ValueTypeData(type, null) : new ValueTypeData(VisLang.ValueType.Array, type));

    private static readonly Parser<VisLang.PrintNode> _printFunctionParser = (
        from keyword in Parse.String("print").Token()
        from lb in Parse.Char('(').Token()
        from content in (_quotedText.Token().Select(p => new VisLang.VariableGetConstNode() { Value = new VisLang.Value(new ValueTypeData(VisLang.ValueType.String), p) })
                            .Or<VisLang.DataNode>(_expression)
                            .Or<VisLang.DataNode>(_operand)).Named("string or variable name")
        from rb in Parse.Char(')').Token()
        from end in Parse.Char(';').Token()
        select new VisLang.PrintNode() { Inputs = new() { content } }
    );

    private static readonly Parser<VisLang.DataNode> _operand = _variableName.Token().Select(p => new VariableGetNode() { Name = p })
                    .Or<VisLang.DataNode>(Parse.DecimalInvariant.Or(Parse.Number).Select(num => new VariableGetConstNode() { Value = new Value(new ValueTypeData(ValueType.Float), float.Parse(num)) }));


    private static DataNode MakeBinaryExpression(OperationType op, DataNode left, DataNode right)
    {
        switch (op)
        {
            case OperationType.Add:
                return new AdditionNode()
                {
                    Inputs = new()
                        {
                            left,
                            right
                        }
                };
            case OperationType.Sub:
                return new SubtractionNode()
                {
                    Inputs = new()
                        {
                            left,
                            right
                        }
                };
            case OperationType.Mul:
                return new MultiplicationNode()
                {
                    Inputs = new()
                        {
                            left,
                            right
                        }
                };
            case OperationType.Div:
                return new DivisionNode()
                {
                    Inputs = new()
                        {
                            left,
                            right
                        }
                };

        }
        return null;
    }

    private static readonly Parser<VisLang.DataNode> _factor = (
        from lp in Parse.Char('(').Token()
        from expr in Parse.Ref(() => _expression)
        from rp in Parse.Char(')').Token()
        select expr).Named("Expression").Or(_operand);

    private static readonly Parser<VisLang.DataNode> _exprOperand = (
        from sign in Parse.Char('-')
        from factor in _factor
        select new VisLang.UnaryNegateOperationNodeBase()
        {
            Inputs = new()
            {
                factor
            }
        }
    ).Or(_factor);

    private static readonly Parser<VisLang.DataNode> _expressionTerm = Parse.ChainOperator(OperationTypeParser.ParseMul.Or(OperationTypeParser.ParseDiv), _exprOperand, MakeBinaryExpression);

    private static readonly Parser<VisLang.DataNode> _expression = Parse.ChainOperator<VisLang.DataNode, OperationType>(
        OperationTypeParser.ParseAdd.Or(OperationTypeParser.ParseSub), _expressionTerm, MakeBinaryExpression
    );

    private static readonly Parser<VisLang.VariableSetNode> _assignmentParser = (
        from name in _variableName.Token()
        from ass in Parse.Char('=').Token()
        from other in _quotedText.Select(p => new VariableGetConstNode() { Value = new Value(new ValueTypeData(ValueType.String), p) }).Or(_expression)
        from end in Parse.Char(';').Token()
        select new VisLang.VariableSetNode()
        {
            Name = name,
            Inputs = new()
            {
                other
            }
        }
    );

    private static readonly Parser<VariableInitInfo> _variableInit =
    (
        from keyword in Parse.String("let").Token()
            // variable name must start with a better or '_' and can end in number or char
        from name in _variableName.Token()
        from colon in Parse.Char(':').Token()
        from type in _typeParser
        from ending in Parse.Char(';').Token()
        select new VariableInitInfo(name, type)
    );

    private static readonly Parser<ParsedProgramInfo> _baseProgram =
    (
        from keyword in Parse.String("program").Token()
        from name in Parse.Letter.AtLeastOnce().Text().Token().Optional()
        from lb in Parse.Char('{').Token()
        from vars in _variableInit.XMany()
        from functions in
            (
                _printFunctionParser.Or<VisLang.ExecutionNode>(_assignmentParser)
            ).XMany()
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
        ParsedProgramInfo program = _baseProgram.Parse(code);
        VisLang.VisSystem system = new VisLang.VisSystem();
        foreach (VariableInitInfo info in program.Variables)
        {
            system.VisSystemMemory.CreateVariable(info.Name, info.Type, null);
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