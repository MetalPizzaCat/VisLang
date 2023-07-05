using Sprache;
using TextLang.Parser;


public static class Program
{
    public static void Main(string[] args)
    {
        string code = @"
program{
    let a : int;
    let b : int;
    let hello : string;
    let array : int[];
    b = -9.9;
    print(b);
    b = 5 - -(b * b);
    b = 6.66;
    print(-(b * b));
}
        ";
        try
        {

            VisLang.VisSystem? vars = VisLangParser.ParseCodeNoComments(code);
            vars.OnOutputAdded += (string text) => Console.WriteLine(text);
            vars.Execute();
        }
        catch (Sprache.ParseException e)
        {
            string[] lines = code.Split(Environment.NewLine);
            Console.Error.WriteLine($"Code parsing error: {e.Message}");
            for (int i = -2; i < 0; i++)
            {
                if (lines.ElementAtOrDefault(e.Position.Line + i - 1) != null)
                {
                    Console.Error.WriteLine($"{e.Position.Line + i}: {lines.ElementAtOrDefault(e.Position.Line + i - 1)}");
                }
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine($"{e.Position.Line}: {lines[e.Position.Line - 1]}");
            Console.Error.WriteLine($"{new string(' ', e.Position.Column + 2)}~~~");
            Console.ResetColor();
            Console.Error.WriteLine($"{e.Position.Line + 1}: {lines.ElementAtOrDefault(e.Position.Line)}");

        }
    }
}