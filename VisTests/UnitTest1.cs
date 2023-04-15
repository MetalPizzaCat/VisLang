using VisLang;
namespace VisTests;

[TestClass]
public class UnitTest1
{
    /// <summary>
    /// Test if execution system even works
    /// </summary>
    [TestMethod]
    public void TestPrint()
    {
        VisSystem system = new VisSystem();
        system.Entrance = new PrintNode(system);

        system.Execute();
        Assert.AreEqual(1, system.Output.Count);
    }

    /// <summary>
    /// Test if we can create a variable in the system and then change it 
    /// </summary>
    [TestMethod]
    public void TestVariableSet()
    {
        VisSystem system = new VisSystem();
        system.VisSystemMemory.CreateVariable("i", VariableType.Number, 0);
        Assert.IsNotNull(system.VisSystemMemory["i"]);
        Assert.AreEqual(0, system.VisSystemMemory["i"].Value.Data);
        system.Entrance = new VariableSetNode(system) { Name = "i", DefaultValue = 69f };

        system.Execute();
        Assert.AreEqual(69f, system.VisSystemMemory["i"].Value.Data);
    }

    /// <summary>
    /// Test if we can create two variables in the system and then set one to another
    /// </summary>
    [TestMethod]
    public void TestVariableGetSet()
    {
        VisSystem system = new VisSystem();
        system.VisSystemMemory.CreateVariable("i", VariableType.Number, 0);
        system.VisSystemMemory.CreateVariable("j", VariableType.Number, 85f);
        Assert.IsNotNull(system.VisSystemMemory["i"]);
        Assert.IsNotNull(system.VisSystemMemory["j"]);
        Assert.AreEqual(0, system.VisSystemMemory["i"].Value.Data);
        Assert.AreEqual(85f, system.VisSystemMemory["j"].Value.Data);
        VariableSetNode setter = new VariableSetNode(system) { Name = "i", DefaultValue = 69f };
        setter.Inputs.Add(new VariableGetNode(system) { Name = "j" });
        system.Entrance = setter;


        system.Execute();
        Assert.AreNotEqual(69f, system.VisSystemMemory["i"].Value.Data);
        Assert.AreEqual(85f, system.VisSystemMemory["i"].Value.Data);
    }
}