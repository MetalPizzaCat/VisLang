using Sprache;
using TextLang.Parser;


public static class Program
{
    public static void Main(string[] args)
    {
        string code = @"program{
            let a : int
            let b : int
            let hello : string
            let array : int[]
            b = -9
            print(b)
            b = 5 - -(b * b)
            print(b)
        }";
        VisLang.VisSystem? vars = VisLangParser.ParseCodeNoComments(code);
        vars.OnOutputAdded += (string text) => Console.WriteLine(text);
        vars.Execute();
    }
}