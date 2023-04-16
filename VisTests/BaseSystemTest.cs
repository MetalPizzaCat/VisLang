using VisLang;
namespace VisTests;

[TestClass]
public class BasicSystemTest
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
        system.VisSystemMemory.CreateVariable("i", VisLang.ValueType.Number, 0);
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
        system.VisSystemMemory.CreateVariable("i", VisLang.ValueType.Number, 0);
        system.VisSystemMemory.CreateVariable("j", VisLang.ValueType.Number, 85f);
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

    /// <summary>
    /// Test if we can print value of created variable
    /// </summary>
    [TestMethod]
    public void TestVariablePrint()
    {
        VisSystem system = new VisSystem();
        system.VisSystemMemory.CreateVariable("i", VisLang.ValueType.Number, 99f);
        Assert.IsNotNull(system.VisSystemMemory["i"]);
        Assert.AreEqual(99f, system.VisSystemMemory["i"].Value.Data);
        PrintNode printer = new PrintNode(system);
        printer.Inputs.Add(new VariableGetNode(system) { Name = "i" });
        system.Entrance = printer;

        system.Execute();
        Assert.AreEqual(1, system.Output.Count);
        Assert.AreEqual(99f.ToString(), system.Output.First());
    }

    /// <summary>
    /// Test if we can print value of created variable
    /// </summary>
    [TestMethod]
    public void TestControlFlow()
    {
        VisSystem system = new VisSystem();
        system.VisSystemMemory.CreateVariable("a", VisLang.ValueType.Number, 99f);
        system.VisSystemMemory.CreateVariable("b", VisLang.ValueType.Number, 99f);
        system.VisSystemMemory.CreateVariable("result", VisLang.ValueType.Number, -1f);
        Assert.IsNotNull(system.VisSystemMemory["a"]);
        Assert.IsNotNull(system.VisSystemMemory["b"]);
        Assert.IsNotNull(system.VisSystemMemory["result"]);
        Assert.AreEqual(system.VisSystemMemory["a"].Value.Data, system.VisSystemMemory["b"].Value.Data);

        EqualsNode eq = new EqualsNode(system)
        {
            Inputs = new List<DataNode>()
            {
                 new VariableGetNode(system) { Name = "a" },
                 new VariableGetNode(system) { Name = "b" }
            }
        };

        FlowControlIfNode cond = new FlowControlIfNode(system)
        {
            SuccessNext = new VariableSetNode(system) { Name = "result", DefaultValue = 1f },
            FailureNext = new VariableSetNode(system) { Name = "result", DefaultValue = 0f },
            Inputs = new List<DataNode>() { eq }
        };
        system.Entrance = cond;
        system.Execute();
        Assert.AreEqual(1f, system.VisSystemMemory["result"].Value.Data);
    }
}