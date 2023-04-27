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
        Assert.AreEqual(0, system.VisSystemMemory["i"].Data);
        system.Entrance = new VariableSetNode(system) { Name = "i", DefaultValue = 69f };

        system.Execute();
        Assert.AreEqual(69f, system.VisSystemMemory["i"].Data);
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
        Assert.AreEqual(0, system.VisSystemMemory["i"].Data);
        Assert.AreEqual(85f, system.VisSystemMemory["j"].Data);
        VariableSetNode setter = new VariableSetNode(system) { Name = "i", DefaultValue = 69f };
        setter.Inputs.Add(new VariableGetNode(system) { Name = "j" });
        system.Entrance = setter;


        system.Execute();
        Assert.AreNotEqual(69f, system.VisSystemMemory["i"].Data);
        Assert.AreEqual(85f, system.VisSystemMemory["i"].Data);
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
        Assert.AreEqual(99f, system.VisSystemMemory["i"].Data);
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
        Assert.AreEqual(system.VisSystemMemory["a"].Data, system.VisSystemMemory["b"].Data);

        EqualsNode eq = new EqualsNode(system)
        {
            Inputs = new List<VisNode>()
            {
                 new VariableGetNode(system) { Name = "a" },
                 new VariableGetNode(system) { Name = "b" }
            }
        };

        FlowControlIfNode cond = new FlowControlIfNode(system)
        {
            SuccessNext = new VariableSetNode(system) { Name = "result", DefaultValue = 1f },
            FailureNext = new VariableSetNode(system) { Name = "result", DefaultValue = 0f },
            Inputs = new List<VisNode>() { eq }
        };
        system.Entrance = cond;
        system.Execute();
        Assert.AreEqual(1f, system.VisSystemMemory["result"].Data);
    }

    /// <summary>
    /// Test if we can get address of a variable
    /// </summary>
    [TestMethod]
    public void TestVariablePrintAddress()
    {
        VisSystem system = new VisSystem();
        system.VisSystemMemory.CreateVariable("i", VisLang.ValueType.Number, 99f);
        Assert.IsNotNull(system.VisSystemMemory["i"]);
        Assert.AreEqual(99f, system.VisSystemMemory["i"].Data);
        PrintNode printer = new PrintNode(system);
        printer.Inputs.Add(new VariableGetAddressNode(system) { Name = "i" });
        system.Entrance = printer;

        system.Execute();
        Assert.AreEqual(1, system.Output.Count);
        Assert.AreEqual(1.ToString(), system.Output.First());
    }

    /// <summary>
    /// Test if we can print value of created variable
    /// </summary>
    [TestMethod]
    public void TestVariablePrintInvalidAddress()
    {
        VisSystem system = new VisSystem();
        system.VisSystemMemory.CreateVariable("i", VisLang.ValueType.Number, 99f);
        Assert.IsNotNull(system.VisSystemMemory["i"]);
        Assert.AreEqual(99f, system.VisSystemMemory["i"].Data);
        PrintNode printer = new PrintNode(system);
        printer.Inputs.Add(new VariableGetAddressNode(system) { Name = "j" });
        system.Entrance = printer;

        system.Execute();
        Assert.AreEqual(1, system.Output.Count);
        Assert.AreEqual(0.ToString(), system.Output.First());
    }

    /// <summary>
    /// Tests if we can store address of a variable in a variable
    /// </summary>
    [TestMethod]
    public void TestVariablePrintAddressOfAddress()
    {
        VisSystem system = new VisSystem();
        system.VisSystemMemory.CreateVariable("i", VisLang.ValueType.Number, 99f);
        system.VisSystemMemory.CreateVariable("i_ptr", VisLang.ValueType.Address, 0);
        Assert.IsNotNull(system.VisSystemMemory["i"]);
        Assert.AreEqual(99f, system.VisSystemMemory["i"].Data);
        Assert.IsNotNull(system.VisSystemMemory["i_ptr"]);
        Assert.AreEqual(0, system.VisSystemMemory["i_ptr"].Data);
        VariableSetNode setAddr = new VariableSetNode(system)
        {
            Name = "i_ptr",
            Inputs = new List<VisNode>()
            {
                 new VariableGetAddressNode(system) { Name = "i" }
            }
        };

        PrintNode printer = new PrintNode(system)
        {
            DefaultNext = setAddr
        };
        printer.Inputs.Add(new VariableGetAddressNode(system) { Name = "i" });

        system.Entrance = printer;

        system.Execute();
        Assert.AreEqual(1, system.Output.Count);
        Assert.AreEqual(1.ToString(), system.Output.First());
        Assert.AreEqual(system.VisSystemMemory["i"].Address, system.VisSystemMemory["i_ptr"].Data);
    }

    /// <summary>
    /// Create a dynamic value and print it's address and value
    /// </summary>
    [TestMethod]
    public void TestAllocate()
    {
        VisSystem system = new VisSystem();
        PrintNode printer = new PrintNode(system);
        VariableAllocateNode alloc = new VariableAllocateNode(system)
        {
            ValueType = VisLang.ValueType.String,
            DefaultValue = "your mom",
            DefaultNext = printer
        };
        printer.Inputs.Add(new ValueGetDereferencedNode(system)
        {
            Inputs = new List<VisNode>()
            {
                alloc
            }
        });
        system.Entrance = alloc;

        system.Execute();
        Assert.IsTrue(system.VisSystemMemory.Memory.ContainsKey(1));
        Assert.AreEqual(1, system.Output.Count);
        Assert.AreEqual("your mom", system.Output.First());
    }

    /// <summary>
    /// Test if we can print value of created variable
    /// </summary>
    [TestMethod]
    public void TestGetAddress()
    {
        VisSystem system = new VisSystem();
        system.VisSystemMemory.CreateVariable("i", VisLang.ValueType.Address, 0);
        Assert.IsNotNull(system.VisSystemMemory["i"]);
        Assert.AreEqual(0, system.VisSystemMemory["i"].Data);
        PrintNode printer = new PrintNode(system);
        VariableAllocateNode alloc = new VariableAllocateNode(system)
        {
            ValueType = VisLang.ValueType.String,
            DefaultValue = "your mom",
            DefaultNext = printer
        };
        printer.Inputs.Add(new ValueGetDereferencedNode(system)
        {
            Inputs = new List<VisNode>()
            {
                alloc
            }
        });
        VariableSetNode setAddr = new VariableSetNode(system)
        {
            Name = "i",
            Inputs = new()
            {
                alloc
            }
        };
        printer.DefaultNext = setAddr;
        system.Entrance = alloc;

        system.Execute();
        Assert.IsTrue(system.VisSystemMemory.Memory.ContainsKey(1));
        Assert.AreEqual(1, system.Output.Count);
        Assert.AreEqual(2u, system.VisSystemMemory["i"].Data);
        Assert.AreEqual("your mom", system.Output.First());
    }
}