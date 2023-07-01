namespace TextLang.Parser;
using Sprache;


public enum OperationType
{
    Add,
    Sub,
    Mul,
    Div
}


public static class OperationTypeParser
{
    public static Parser<OperationType> ParseOperationType = Parse.Char('+').Token().Return(OperationType.Add)
                                                                    .Or(Parse.Char('-').Token().Return(OperationType.Sub))
                                                                    .Or(Parse.Char('/').Token().Return(OperationType.Div))
                                                                    .Or(Parse.Char('*').Token().Return(OperationType.Mul));

    public static Parser<OperationType> ParseAdd = Parse.Char('+').Token().Return(OperationType.Add);
    public static Parser<OperationType> ParseSub = Parse.Char('-').Token().Return(OperationType.Sub);
    public static Parser<OperationType> ParseDiv = Parse.Char('/').Token().Return(OperationType.Div);
    public static Parser<OperationType> ParseMul = Parse.Char('*').Token().Return(OperationType.Mul);
}