using VisLang;
namespace VisTests;
using TextLang;

[TestClass]
public class TextLangTests
{
    [TestMethod]
    public void BasicParsing()
    {
        string code = @"program{}";
        Assert.IsNotNull(TextLang.Parser.VisLangParser.ParseCodeNoComments(code));
    }
    [TestMethod]
    public void BasicParsingVariableDeclaration()
    {
        string code = @"program{
            let a : int;
            let b : float;
            let c : string;
        }";
        VisSystem? system = TextLang.Parser.VisLangParser.ParseCodeNoComments(code);
        Assert.IsNotNull(system);
        Assert.IsNotNull(system.VisSystemMemory["a"]);
        Assert.IsNotNull(system.VisSystemMemory["b"]);
        Assert.IsNotNull(system.VisSystemMemory["c"]);

        Assert.AreEqual(VisLang.ValueType.Integer, system.VisSystemMemory["a"].ValueType);
        Assert.AreEqual(VisLang.ValueType.Float, system.VisSystemMemory["b"].ValueType);
        Assert.AreEqual(VisLang.ValueType.String, system.VisSystemMemory["c"].ValueType);
    }

    [TestMethod]
    public void BasicParsingVariableDeclarationExtendedNames()
    {
        string code = @"program{
            let a23 : int;
            let ba_3ba : float;
            let c23ss__ : string;
        }";
        VisSystem? system = TextLang.Parser.VisLangParser.ParseCodeNoComments(code);
        Assert.IsNotNull(system);
        Assert.IsNotNull(system.VisSystemMemory["a23"]);
        Assert.IsNotNull(system.VisSystemMemory["ba_3ba"]);
        Assert.IsNotNull(system.VisSystemMemory["c23ss__"]);

        Assert.AreEqual(VisLang.ValueType.Integer, system.VisSystemMemory["a23"].ValueType);
        Assert.AreEqual(VisLang.ValueType.Float, system.VisSystemMemory["ba_3ba"].ValueType);
        Assert.AreEqual(VisLang.ValueType.String, system.VisSystemMemory["c23ss__"].ValueType);
    }

    [TestMethod]
    public void TestAssignment()
    {
        string code = @"program{
            let a : float;
            let b : float;
            a = 2;
            b = a;
        }";
        VisSystem? system = TextLang.Parser.VisLangParser.ParseCodeNoComments(code);
        system.Execute();
        Assert.IsNotNull(system);
        Assert.IsNotNull(system.VisSystemMemory["a"]);
        Assert.IsNotNull(system.VisSystemMemory["b"]);

        Assert.AreEqual(VisLang.ValueType.Float, system.VisSystemMemory["a"].ValueType);
        Assert.AreEqual(VisLang.ValueType.Float, system.VisSystemMemory["b"].ValueType);

        Assert.AreEqual((float)2, system.VisSystemMemory["a"].Data);
        Assert.AreEqual((float)2, system.VisSystemMemory["b"].Data);
        Assert.AreEqual(system.VisSystemMemory["a"].Data, system.VisSystemMemory["b"].Data);
    }

    [TestMethod]
    public void TestAssignmentWithMath()
    {
        string code = @"program{
            let a : float;
            let b : float;
            let c : float;
            a = 2;
            b = a*a;
            c = -a + a;
        }";
        VisSystem? system = TextLang.Parser.VisLangParser.ParseCodeNoComments(code);
        system.Execute();
        Assert.IsNotNull(system);
        Assert.IsNotNull(system.VisSystemMemory["a"]);
        Assert.IsNotNull(system.VisSystemMemory["b"]);
        Assert.IsNotNull(system.VisSystemMemory["c"]);

        Assert.AreEqual(VisLang.ValueType.Float, system.VisSystemMemory["a"].ValueType);
        Assert.AreEqual(VisLang.ValueType.Float, system.VisSystemMemory["b"].ValueType);
        Assert.AreEqual(VisLang.ValueType.Float, system.VisSystemMemory["c"].ValueType);

        Assert.AreEqual((float)2, system.VisSystemMemory["a"].Data);
        Assert.AreEqual((float)4, system.VisSystemMemory["b"].Data);
        Assert.AreEqual((float)0, system.VisSystemMemory["c"].Data);
        Assert.AreNotEqual(system.VisSystemMemory["a"].Data, system.VisSystemMemory["b"].Data);
    }
}