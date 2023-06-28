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

            print('hello')
            print('testing testing')
            print('a: ')
            print(a)
            print('b:')
            print(b)
            print('array: ')
            print(array)
        }";
        VisLang.VisSystem? vars = VisLangParser.ParseCodeNoComments(code);
        vars.OnOutputAdded += (string text) => Console.WriteLine(text);
        vars.Execute();
    }
}