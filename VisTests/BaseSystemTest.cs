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
        system.VisSystemMemory.CreateVariable("i", VisLang.ValueType.Float, 0);
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
        system.VisSystemMemory.CreateVariable("i", VisLang.ValueType.Float, 0);
        system.VisSystemMemory.CreateVariable("j", VisLang.ValueType.Float, 85f);
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
        system.VisSystemMemory.CreateVariable("i", VisLang.ValueType.Float, 99f);
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
        system.VisSystemMemory.CreateVariable("a", VisLang.ValueType.Float, 99f);
        system.VisSystemMemory.CreateVariable("b", VisLang.ValueType.Float, 99f);
        system.VisSystemMemory.CreateVariable("result", VisLang.ValueType.Float, -1f);
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
        system.VisSystemMemory.CreateVariable("i", VisLang.ValueType.Float, 99f);
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
        system.VisSystemMemory.CreateVariable("i", VisLang.ValueType.Float, 99f);
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
        system.VisSystemMemory.CreateVariable("i", VisLang.ValueType.Float, 99f);
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

    [TestMethod]
    public void TestProcedureCallPrint()
    {
        VisSystem system = new VisSystem();
        system.Procedures.Add(new VisProcedure(system)
        {
            Name = "TestLolYourMom",
            Arguments = new(),
            OutputValueType = null,
        });
        VisProcedure? proc = system.GetProcedure("TestLolYourMom");
        Assert.IsNotNull(proc);
        proc.ProcedureNodesRoot = new PrintNode(proc.SubSystem)
        {
            Inputs = new List<VisNode>()
                {
                    new VariableGetConstNode()
                    {
                        Value = new Value(VisLang.ValueType.String,"Print inside procedure!")
                    }
                }
        };
        system.Entrance = new ProcedureCallNode(system) { ProcedureName = "TestLolYourMom", DefaultNext = new ProcedureCallNode(system) { ProcedureName = "TestLolYourMom" } };
        system.Execute();
        Assert.AreEqual(2, system.Output.Count);
        Assert.AreEqual("Print inside procedure!", system.Output.FirstOrDefault());
    }

    [TestMethod]
    public void TestProcedureCallPrintArgument()
    {
        VisSystem system = new VisSystem();
        system.Procedures.Add(new VisProcedure(system)
        {
            Name = "TestLolYourMom",
            Arguments = new() { { "arg_text", VisLang.ValueType.String } },
            OutputValueType = null,
        });
        VisProcedure? proc = system.GetProcedure("TestLolYourMom");
        Assert.IsNotNull(proc);
        proc.ProcedureNodesRoot = new PrintNode(proc.SubSystem)
        {
            Inputs = new List<VisNode>()
                {
                    new VariableGetNode(proc.SubSystem)
                    {
                        Name = "arg_text"
                    }
                }
        };
        system.Entrance = new ProcedureCallNode(system)
        {
            ProcedureName = "TestLolYourMom",
            Inputs = new() { new VariableGetConstNode() { Value = new Value(VisLang.ValueType.String, "Basinga!") } },
            DefaultNext = new ProcedureCallNode(system)
            {
                ProcedureName = "TestLolYourMom",
                Inputs = new() { new VariableGetConstNode() { Value = new Value(VisLang.ValueType.String, "Lol no") } },
            }
        };
        system.Execute();
        Assert.AreEqual(2, system.Output.Count);
        Assert.AreEqual("Basinga!", system.Output.FirstOrDefault());
        Assert.AreEqual("Lol no", system.Output.ElementAtOrDefault(1));
    }

    [TestMethod]
    public void TestProcedureCustomDataNode()
    {
        VisSystem system = new VisSystem();
        system.Procedures.Add(new VisProcedure(system)
        {
            Name = "AddOne",
            Arguments = new() { { "arg_num", VisLang.ValueType.Float } },
            OutputValueType = null,
        });
        VisProcedure? proc = system.GetProcedure("AddOne");
        Assert.IsNotNull(proc);
        proc.ProcedureNodesRoot = new PrintNode(proc.SubSystem)
        {
            Inputs = new List<VisNode>()
                {
                    new VariableGetNode(proc.SubSystem)
                    {
                        Name = "arg_num"
                    }
                }
        };
        system.Entrance = new ProcedureCallNode(system)
        {
            ProcedureName = proc.Name,
            Inputs = new() { new VariableGetConstNode() { Value = new Value(VisLang.ValueType.String, "Basinga!") } },
            DefaultNext = new ProcedureCallNode(system)
            {
                ProcedureName = proc.Name,
                Inputs = new() { new VariableGetConstNode() { Value = new Value(VisLang.ValueType.String, "Lol no") } },
            }
        };
        system.Execute();
        Assert.AreEqual(2, system.Output.Count);
        Assert.AreEqual("Basinga!", system.Output.FirstOrDefault());
        Assert.AreEqual("Lol no", system.Output.ElementAtOrDefault(1));
    }

    [TestMethod]
    public void TestFunctionDoMathWithArgument()
    {
        VisSystem system = new VisSystem();
        system.Functions.Add(new VisFunction(system)
        {
            Name = "CoolMath",
            Arguments = new() { { "arg", VisLang.ValueType.Float } },
            OutputValueType = VisLang.ValueType.Float
        });

        VisFunction? func = system.GetFunction("CoolMath");
        Assert.IsNotNull(func);
        func.Root = new AdditionNode(func.SubSystem)
        {
            Inputs = new()
            {
                new VariableGetNode(func.SubSystem){Name = "arg"},
                new VariableGetNode(func.SubSystem){Name = "arg"}
            }
        };
        system.Entrance = new PrintNode(system)
        {
            Inputs = new()
            {
                new FunctionCallNode(system)
                {
                    FunctionName = "CoolMath",
                    Inputs = new()
                    {
                        new VariableGetConstNode(){Value = new Value(VisLang.ValueType.Float,3)}
                    }
                }
            }
        };
        system.Execute();
        Assert.AreEqual(1, system.Output.Count);
        Assert.AreEqual("6", system.Output.FirstOrDefault());
    }

    [TestMethod]
    public void TestArrayAppend()
    {
        VisSystem system = new VisSystem();
        system.VisSystemMemory.CreateVariable("arr", VisLang.ValueType.Array);
        Assert.IsNotNull(system.VisSystemMemory["arr"]);
        Assert.IsTrue(system?.VisSystemMemory["arr"]?.Data is List<Value>);
        system.Entrance = new ArrayAppendElement(system)
        {
            Inputs = new()
            {
                // array
                new VariableGetNode(system)
                {
                    Name = "arr"
                },
                // value
                new VariableGetConstNode(){Value = new Value(VisLang.ValueType.Float ,69f)}
            }
        };

        system.Execute();
        Assert.AreEqual(69f, (system?.VisSystemMemory["arr"]?.Data as List<Value>)?.ElementAt(0)?.Data ?? 0);
        //Assert.AreEqual(69f, system.VisSystemMemory["i"].Data);
    }

    [TestMethod]
    public void TestArrayAppendAndGet()
    {
        VisSystem system = new VisSystem();
        system.VisSystemMemory.CreateVariable("arr", VisLang.ValueType.Array);
        system.VisSystemMemory.CreateVariable("res", VisLang.ValueType.Float, 0);
        Assert.IsNotNull(system.VisSystemMemory["arr"]);
        Assert.IsNotNull(system.VisSystemMemory["res"]);
        Assert.IsTrue(system?.VisSystemMemory["arr"]?.Data is List<Value>);
        system.Entrance = new ArrayAppendElement(system)
        {
            Inputs = new()
            {
                // array
                new VariableGetNode(system)
                {
                    Name = "arr"
                },
                // value
                new VariableGetConstNode(){Value = new Value(VisLang.ValueType.Float ,69f)}
            },
            DefaultNext = new VariableSetNode(system)
            {
                Name = "res",
                Inputs = new()
                {
                    new ArrayGetElementAtNode(system)
                    {
                        Inputs = new List<VisNode>()
                        {
                            // array
                            new VariableGetNode(system)
                            {
                                Name = "arr"
                            },
                            // value
                            new VariableGetConstNode(){Value = new Value(VisLang.ValueType.Integer ,0)}
                        }
                    }
                }
            }
        };

        system.Execute();
        Assert.AreEqual(69f, (system?.VisSystemMemory["arr"]?.Data as List<Value>)?.ElementAt(0)?.Data ?? 0);
        Assert.AreEqual(69f, system?.VisSystemMemory["res"]?.Data);
        //Assert.AreEqual(69f, system.VisSystemMemory["i"].Data);
    }

    [TestMethod]
    public void TestArrayAppendAndGetAndThenSet()
    {
        VisSystem system = new VisSystem();
        system.VisSystemMemory.CreateVariable("arr", VisLang.ValueType.Array);
        system.VisSystemMemory.CreateVariable("res", VisLang.ValueType.Float, 0);
        system.VisSystemMemory.CreateVariable("res2", VisLang.ValueType.Float, 0);
        Assert.IsNotNull(system.VisSystemMemory["arr"]);
        Assert.IsNotNull(system.VisSystemMemory["res"]);
        Assert.IsNotNull(system.VisSystemMemory["res2"]);
        Assert.IsTrue(system?.VisSystemMemory["arr"]?.Data is List<Value>);
        system.Entrance = new ArrayAppendElement(system)
        {
            Inputs = new()
            {
                // array
                new VariableGetNode(system)
                {
                    Name = "arr"
                },
                // value
                new VariableGetConstNode(){Value = new Value(VisLang.ValueType.Float ,69f)}
            },
            DefaultNext = new VariableSetNode(system)
            {
                Name = "res",
                Inputs = new()
                {
                    new ArrayGetElementAtNode(system)
                    {
                        Inputs = new List<VisNode>()
                        {
                            // array
                            new VariableGetNode(system)
                            {
                                Name = "arr"
                            },
                            // index
                            new VariableGetConstNode(){Value = new Value(VisLang.ValueType.Integer ,0)}
                        }
                    }
                },
                DefaultNext = new ArraySetElementAtNode(system)
                {
                    Inputs = new()
                    {
                        new VariableGetNode(system)
                        {
                            Name = "arr"
                        },
                        // index
                        new VariableGetConstNode(){Value = new Value(VisLang.ValueType.Integer ,0)},
                         // value
                        new VariableGetConstNode(){Value = new Value(VisLang.ValueType.Float ,420f)}
                    },
                    DefaultNext = new VariableSetNode(system)
                    {
                        Name = "res2",
                        Inputs = new()
                        {
                            new ArrayGetElementAtNode(system)
                            {
                                Inputs = new List<VisNode>()
                                {
                                    // array
                                    new VariableGetNode(system)
                                    {
                                        Name = "arr"
                                    },
                                    // index
                                    new VariableGetConstNode(){Value = new Value(VisLang.ValueType.Integer ,0)}
                                }
                            }
                        },
                    }
                }
            }
        };

        system.Execute();
        Assert.AreEqual(420f, (system?.VisSystemMemory["arr"]?.Data as List<Value>)?.ElementAt(0)?.Data ?? 0);
        Assert.AreEqual(69f, system?.VisSystemMemory["res"]?.Data);
        Assert.AreEqual(420f, system?.VisSystemMemory["res2"]?.Data);
        //Assert.AreEqual(69f, system.VisSystemMemory["i"].Data);
    }

    [TestMethod]
    public void TestArrayAppendAnotherArray()
    {
        VisSystem system = new VisSystem();
        system.VisSystemMemory.CreateVariable("arr", VisLang.ValueType.Array);
        system.VisSystemMemory.CreateVariable("arr2", VisLang.ValueType.Array);
        Assert.IsNotNull(system.VisSystemMemory["arr"]);
        Assert.IsNotNull(system.VisSystemMemory["arr2"]);
        Assert.IsTrue(system?.VisSystemMemory["arr"]?.Data is List<Value>);
        Assert.IsTrue(system?.VisSystemMemory["arr2"]?.Data is List<Value>);
        system.Entrance = new ArrayAppendElement(system)
        {
            Inputs = new()
            {
                // array
                new VariableGetNode(system)
                {
                    Name = "arr"
                },
                // value
                new VariableGetNode(system){Name = "arr2"}
            }
        };

        system.Execute();
        Assert.IsTrue((system?.VisSystemMemory["arr"]?.Data as List<Value>)?.ElementAt(0)?.Data is List<Value>);
        Assert.AreEqual(system?.VisSystemMemory["arr2"]?.Data, (system?.VisSystemMemory["arr"]?.Data as List<Value>)?.ElementAt(0)?.Data);
        Assert.AreNotEqual(system?.VisSystemMemory["arr2"], (system?.VisSystemMemory["arr"]?.Data as List<Value>)?.ElementAt(0));
        Assert.AreEqual("[[]]", system?.VisSystemMemory["arr"]?.TryAsString());
        //Assert.AreEqual(69f, system.VisSystemMemory["i"].Data);
    }
}