using VisLang;
namespace VisTests;

[TestClass]
public class MathTests
{
    /// <summary>
    /// Test if we can set value of variable to sum of two constants
    /// </summary>
    [TestMethod]
    public void TestAddConst()
    {
        VisSystem system = new VisSystem();
        system.VisSystemMemory.CreateVariable("i", VariableType.Number, 0);
        Assert.IsNotNull(system.VisSystemMemory["i"]);
        Assert.AreEqual(0, system.VisSystemMemory["i"].Value.Data);
        VariableSetNode setter = new VariableSetNode(system) { Name = "i", DefaultValue = 69f };
        setter.Inputs.Add(new AdditionNode(system) { DefaultValueLeft = 2, DefaultValueRight = 3 });
        system.Entrance = setter;


        system.Execute();
        Assert.AreNotEqual(69f, system.VisSystemMemory["i"].Value.Data);
        Assert.AreEqual(5f, system.VisSystemMemory["i"].Value.Data);
    }

    /// <summary>
    /// Test if we can set value of variable to sum of a variable and constant
    /// </summary>
    [TestMethod]
    public void TestAddVariableConst()
    {
        VisSystem system = new VisSystem();
        system.VisSystemMemory.CreateVariable("i", VariableType.Number, 0);
        system.VisSystemMemory.CreateVariable("a", VariableType.Number, 6f);
        Assert.IsNotNull(system.VisSystemMemory["i"]);
        Assert.IsNotNull(system.VisSystemMemory["a"]);
        Assert.AreEqual(0, system.VisSystemMemory["i"].Value.Data);
        Assert.AreEqual(6f, system.VisSystemMemory["a"].Value.Data);
        VariableSetNode setter = new VariableSetNode(system) { Name = "i", DefaultValue = 69f };
        AdditionNode adder = new AdditionNode(system) { DefaultValueLeft = 69, DefaultValueRight = 3 };
        adder.Inputs.Add(new VariableGetNode(system) { Name = "a" });
        setter.Inputs.Add(adder);
        system.Entrance = setter;


        system.Execute();
        Assert.AreNotEqual(69f, system.VisSystemMemory["i"].Value.Data);
        Assert.AreEqual(9f, system.VisSystemMemory["i"].Value.Data);
    }

    /// <summary>
    /// Test if we can set value of variable to sum of two variables
    /// </summary>
    [TestMethod]
    public void TestAddVariableVariable()
    {
        VisSystem system = new VisSystem();
        system.VisSystemMemory.CreateVariable("i", VariableType.Number, 0);
        system.VisSystemMemory.CreateVariable("a", VariableType.Number, 6f);
        system.VisSystemMemory.CreateVariable("b", VariableType.Number, 6f);
        Assert.IsNotNull(system.VisSystemMemory["i"]);
        Assert.IsNotNull(system.VisSystemMemory["a"]);
        Assert.IsNotNull(system.VisSystemMemory["b"]);
        Assert.AreEqual(0, system.VisSystemMemory["i"].Value.Data);
        Assert.AreEqual(6f, system.VisSystemMemory["a"].Value.Data);
        Assert.AreEqual(6f, system.VisSystemMemory["b"].Value.Data);
        VariableSetNode setter = new VariableSetNode(system) { Name = "i", DefaultValue = 69f };
        AdditionNode adder = new AdditionNode(system) { DefaultValueLeft = 69, DefaultValueRight = 3 };
        adder.Inputs.Add(new VariableGetNode(system) { Name = "a" });
        adder.Inputs.Add(new VariableGetNode(system) { Name = "b" });
        setter.Inputs.Add(adder);
        system.Entrance = setter;


        system.Execute();
        Assert.AreNotEqual(69f, system.VisSystemMemory["i"].Value.Data);
        Assert.AreEqual(12f, system.VisSystemMemory["i"].Value.Data);
    }

    /// <summary>
    /// Test if we can set value of variable to be variable * 2
    /// </summary>
    [TestMethod]
    public void TestAddVariableSelf()
    {
        VisSystem system = new VisSystem();
        system.VisSystemMemory.CreateVariable("i", VariableType.Number, 1f);
        Assert.IsNotNull(system.VisSystemMemory["i"]);
        Assert.AreEqual(1f, system.VisSystemMemory["i"].Value.Data);
        VariableSetNode setter = new VariableSetNode(system) { Name = "i", DefaultValue = 69f };
        AdditionNode adder = new AdditionNode(system) { DefaultValueLeft = 69, DefaultValueRight = 3 };
        adder.Inputs.Add(new VariableGetNode(system) { Name = "i" });
        adder.Inputs.Add(new VariableGetNode(system) { Name = "i" });
        setter.Inputs.Add(adder);
        system.Entrance = setter;


        system.Execute();
        Assert.AreNotEqual(69f, system.VisSystemMemory["i"].Value.Data);
        Assert.AreEqual(2f, system.VisSystemMemory["i"].Value.Data);
    }
}