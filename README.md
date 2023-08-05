# VisLang
VisLang is an interpreted language inspired by Unreal Engine blueprints, Godot's visual code and Fortran.
VisLang itself is a simple C# library that contains all the basics needed to execute code, with Editor for the language created using Godot 4 that translates user created visual nodes into executable objects.
![image](https://github.com/MetalPizzaCat/VisLang/assets/36876492/22f83ea0-0a31-4d0f-a8c8-ecf17cc797fd)

## VisLang Interpreter
Interepreter itself is a simple library written in C# that executes it's code by storing the code as a one directional linked list, with each node storing it's logic and pointer to the next node.
### Value
Value represents some amount of data that is stored in the memory, value type can not be changed at runtime. 
Allowed value types are 
* Bool(true or false)
* Char(a single character)
* Interger(64bit int)
* Float(64bit float)
* String(C# String object)
* Array (C# List object)
* Address (32bit unsigned int)
  Arrays are special as they allow user to store any type of value inside if no checks were added
### Node
A node represents a single unit of logic in the language. There are two types of nodes:
* Executable: This node is executed once when interpreter selects it and is designed to perform actions that modify the state
* Data: These nodes can be executed multilple times, but must always return the same data if given the same inputs
## Editor
Editor is written in Godot 4.0 and uses Godot's built in node graph system. It is meant to provide the ability to edit, save, launch and debug code created by the user
## TextLang
TextLang is an expriremental project built usign VisLang library that allows user to write code in a text form before transforming it into executable objects in memory. This project exists as a proof of concept and maybe removed in the future

# Requirements 
To build .Net7 and Godot 4.0 are required
