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
            b = 9
            hello = 'hello world!'
            print('hello')
            print('testing testing')
            print('a:')
            print(a)
            print('b:')
            print(b)
            b = 90
            print(b)
            print('array:')
            print(array)
            print(hello)
        }";
        VisLang.VisSystem? vars = VisLangParser.ParseCodeNoComments(code);
        vars.OnOutputAdded += (string text) => Console.WriteLine(text);
        vars.Execute();
    }
}