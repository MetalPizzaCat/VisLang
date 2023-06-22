# VisLang
VisLang is a simple project created for no reason other then "why not", based on unreal engine blueprint.

It is a simple interpreted programming language inspired by C and FORTRAN in terms of logic and Unreal Engine 4 Blueprints in terms of code representation and writen in C#. While it is meant to be like flow charts and edited via a visual medium, thatt is not a requirment.

## Projects
VisLang project is split in two parts: Interpreter and Editor.
* Interpreter can function on it's own and can execute VisLang code that was originally written in any form as long as it gets converted into c# Node objects. It is written in C# using .NET 7. It defines
  -  Interpreter object that can execute nodes given from outside and store results
  -  A collection of base nodes that can be used to create VisLang executable trees
* Editor provides a way to visually edit VisLang code written in C# using Godot 4. This project requires VisLang base project and provides:
  - Editor
  - Very simple debugger
