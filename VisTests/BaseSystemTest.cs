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
            Inputs = new()
            {
                 new VariableGetNode(system) { Name = "a" },
                 new VariableGetNode(system) { Name = "b" }
            }
        };

        FlowControlIfNode cond = new FlowControlIfNode(system)
        {
            SuccessNext = new VariableSetNode(system) { Name = "result", DefaultValue = 1f },
            FailureNext = new VariableSetNode(system) { Name = "result", DefaultValue = 0f },
            Inputs = new() { eq }
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
            Inputs = new()
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
            Inputs = new()
            {
                new VisLang.Internal.PopItemFromReturnStackNode(system)
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
            Inputs = new()
            {
                new VisLang.Internal.PeekAtItemFromReturnStackNode(system)
            }
        });
        VariableSetNode setAddr = new VariableSetNode(system)
        {
            Name = "i",
            Inputs = new()
            {
                new VisLang.Internal.PopItemFromReturnStackNode(system)
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
        system.Procedures.Add(new VisProcedure()
        {
            Name = "TestLolYourMom",
            Arguments = new(),
            OutputValueType = null,
        });
        VisProcedure? proc = system.GetProcedure("TestLolYourMom");
        Assert.IsNotNull(proc);
        proc.ProcedureNodesRoot = new PrintNode(system)
        {
            Inputs = new()
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
        system.Procedures.Add(new VisProcedure()
        {
            Name = "TestLolYourMom",
            Arguments = new() { { "arg_text", VisLang.ValueType.String } },
            OutputValueType = null,
        });
        VisProcedure? proc = system.GetProcedure("TestLolYourMom");
        Assert.IsNotNull(proc);
        proc.ProcedureNodesRoot = new PrintNode(system)
        {
            Inputs = new()
                {
                    new VariableGetNode(system)
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
        system.Procedures.Add(new VisProcedure()
        {
            Name = "AddOne",
            Arguments = new() { { "arg_num", VisLang.ValueType.String } },
            OutputValueType = null,
        });
        VisProcedure? proc = system.GetProcedure("AddOne");
        Assert.IsNotNull(proc);
        proc.ProcedureNodesRoot = new PrintNode(system)
        {
            Inputs = new()
                {
                    new VariableGetNode(system)
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
    public void TestProcedureCallProcedure()
    {
        /*
            func PrintPlusOne(arg : string){
                print(arg_num);
            }
            func AddOne(arg_num : string){
                print(arg_num);
                PrintPlusOne(arg_num);
            }
            program{
                AddOne("Basinga");
                AddOne("Lol no");
            }
        */
        VisSystem system = new VisSystem();
        system.Procedures.Add(new VisProcedure()
        {
            Name = "AddOne",
            Arguments = new() { { "arg_num", VisLang.ValueType.String } },
            OutputValueType = null,
        });
        system.Procedures.Add(new VisProcedure()
        {
            Name = "PrintPlusOne",
            Arguments = new Dictionary<string, VisLang.ValueType>() { { "arg", VisLang.ValueType.String } },
            OutputValueType = null
        });

        VisProcedure? proc = system.GetProcedure("AddOne");
        VisProcedure? proc2 = system.GetProcedure("PrintPlusOne");
        Assert.IsNotNull(proc);
        Assert.IsNotNull(proc2);
        proc2.ProcedureNodesRoot = new PrintNode(system)
        {
            Inputs = new()
            {
                new VariableGetNode(system)
                    {
                        Name = "arg"
                    }
            }
        };
        proc.ProcedureNodesRoot = new PrintNode(system)
        {
            Inputs = new()
                {
                    new VariableGetNode(system)
                    {
                        Name = "arg_num"
                    }
                },
            DefaultNext = new ProcedureCallNode(system)
            {
                ProcedureName = proc2.Name,
                Inputs = new()
                {
                    new VariableGetNode(system)
                    {
                        Name = "arg_num"
                    }
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
        Assert.AreEqual(4, system.Output.Count);
        Assert.AreEqual("Basinga!", system.Output.FirstOrDefault());
        Assert.AreEqual("Basinga!", system.Output.ElementAtOrDefault(1));
        Assert.AreEqual("Lol no", system.Output.ElementAtOrDefault(2));
        Assert.AreEqual("Lol no", system.Output.ElementAtOrDefault(3));
    }

    [TestMethod]
    public void TestFunctionDoMathWithArgument()
    {
        VisSystem system = new VisSystem();
        system.Functions.Add(new VisFunction()
        {
            Name = "CoolMath",
            Arguments = new() { { "arg", VisLang.ValueType.Float } },
            OutputValueType = VisLang.ValueType.Float
        });

        VisFunction? func = system.GetFunction("CoolMath");
        Assert.IsNotNull(func);
        func.Root = new AdditionNode(system)
        {
            Inputs = new()
            {
                new VariableGetNode(system){Name = "arg"},
                new VariableGetNode(system){Name = "arg"}
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
    public void TestArrayAppendTyped()
    {
        VisSystem system = new VisSystem();
        system.VisSystemMemory.CreateVariable("arr", VisLang.ValueType.Array, null, VisLang.ValueType.Integer);
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

        Assert.ThrowsException<VisLang.Interpreter.ValueTypeMismatchException>(() => system.Execute());
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
                new VariableGetConstNode(){Value = new Value(VisLang.ValueType.Integer ,69)}
            }
        };
        system.Execute();
        Assert.AreEqual(69, (system?.VisSystemMemory["arr"]?.Data as List<Value>)?.ElementAt(0)?.Data ?? 0);
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
                        Inputs = new ()
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
                        Inputs = new ()
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
                                Inputs = new ()
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

    /// <summary>
    /// Test if we can set value of variable to sum of two constants
    /// </summary>
    [TestMethod]
    public void TestAddConst()
    {
        VisSystem system = new VisSystem();
        system.VisSystemMemory.CreateVariable("i", VisLang.ValueType.Float, 0);
        Assert.IsNotNull(system.VisSystemMemory["i"]);
        Assert.AreEqual(0, system.VisSystemMemory["i"].Data);
        VariableSetNode setter = new VariableSetNode(system) { Name = "i", DefaultValue = 69f };
        setter.Inputs.Add(new AdditionNode(system) { DefaultValueLeft = 2, DefaultValueRight = 3 });
        system.Entrance = setter;


        system.Execute();
        Assert.AreNotEqual(69f, system.VisSystemMemory["i"].Data);
        Assert.AreEqual(5f, system.VisSystemMemory["i"].Data);
    }

    /// <summary>
    /// Test if we can set value of variable to sum of a variable and constant
    /// </summary>
    [TestMethod]
    public void TestAddVariableConst()
    {
        VisSystem system = new VisSystem();
        system.VisSystemMemory.CreateVariable("i", VisLang.ValueType.Float, 0);
        system.VisSystemMemory.CreateVariable("a", VisLang.ValueType.Float, 6f);
        Assert.IsNotNull(system.VisSystemMemory["i"]);
        Assert.IsNotNull(system.VisSystemMemory["a"]);
        Assert.AreEqual(0, system.VisSystemMemory["i"].Data);
        Assert.AreEqual(6f, system.VisSystemMemory["a"].Data);
        VariableSetNode setter = new VariableSetNode(system) { Name = "i", DefaultValue = 69f };
        AdditionNode adder = new AdditionNode(system) { DefaultValueLeft = 69, DefaultValueRight = 3 };
        adder.Inputs.Add(new VariableGetNode(system) { Name = "a" });
        setter.Inputs.Add(adder);
        system.Entrance = setter;


        system.Execute();
        Assert.AreNotEqual(69f, system.VisSystemMemory["i"].Data);
        Assert.AreEqual(9f, system.VisSystemMemory["i"].Data);
    }

    /// <summary>
    /// Test if we can set value of variable to sum of two variables
    /// </summary>
    [TestMethod]
    public void TestAddVariableVariable()
    {
        VisSystem system = new VisSystem();
        system.VisSystemMemory.CreateVariable("i", VisLang.ValueType.Float, 0);
        system.VisSystemMemory.CreateVariable("a", VisLang.ValueType.Float, 6f);
        system.VisSystemMemory.CreateVariable("b", VisLang.ValueType.Float, 6f);
        Assert.IsNotNull(system.VisSystemMemory["i"]);
        Assert.IsNotNull(system.VisSystemMemory["a"]);
        Assert.IsNotNull(system.VisSystemMemory["b"]);
        Assert.AreEqual(0, system.VisSystemMemory["i"].Data);
        Assert.AreEqual(6f, system.VisSystemMemory["a"].Data);
        Assert.AreEqual(6f, system.VisSystemMemory["b"].Data);
        VariableSetNode setter = new VariableSetNode(system) { Name = "i", DefaultValue = 69f };
        AdditionNode adder = new AdditionNode(system) { DefaultValueLeft = 69, DefaultValueRight = 3 };
        adder.Inputs.Add(new VariableGetNode(system) { Name = "a" });
        adder.Inputs.Add(new VariableGetNode(system) { Name = "b" });
        setter.Inputs.Add(adder);
        system.Entrance = setter;


        system.Execute();
        Assert.AreNotEqual(69f, system.VisSystemMemory["i"].Data);
        Assert.AreEqual(12f, system.VisSystemMemory["i"].Data);
    }

    /// <summary>
    /// Test if we can set value of variable to be variable * 2
    /// </summary>
    [TestMethod]
    public void TestAddVariableSelf()
    {
        VisSystem system = new VisSystem();
        system.VisSystemMemory.CreateVariable("i", VisLang.ValueType.Float, 1f);
        Assert.IsNotNull(system.VisSystemMemory["i"]);
        Assert.AreEqual(1f, system.VisSystemMemory["i"].Data);
        VariableSetNode setter = new VariableSetNode(system) { Name = "i", DefaultValue = 69f };
        AdditionNode adder = new AdditionNode(system) { DefaultValueLeft = 69, DefaultValueRight = 3 };
        adder.Inputs.Add(new VariableGetNode(system) { Name = "i" });
        adder.Inputs.Add(new VariableGetNode(system) { Name = "i" });
        setter.Inputs.Add(adder);
        system.Entrance = setter;


        system.Execute();
        Assert.AreNotEqual(69f, system.VisSystemMemory["i"].Data);
        Assert.AreEqual(2f, system.VisSystemMemory["i"].Data);
    }
}