using System;
using Godot;
using System.Collections.Generic;
using System.Linq;

public class NodeParser
{
    private List<VisNode> _parsedNodes = new();

    private List<VisLang.VisNode> _visNodes = new();

    private VisNode _root;

    private VisLang.VisSystem _system;

    private List<VisLang.VisNode> GenerateInputs(VisNode root)
    {
        List<VisLang.VisNode> inputs = new();
        foreach (NodeInput input in root.Inputs)
        {
            if (input.Connection == null || input.Connection.OwningNode == null)
            {
                inputs.Add(new VisLang.VariableGetConstNode() { Value = new VisLang.Value(input.InputType, false, input.Data) });
                continue;
            }
            // we will not generate executable functions that are used as input but we will have to connect them somehow
            if (input.Connection.OwningNode?.FunctionInfo?.IsExecutable ?? false)
            {
                // executable can't be referred to before it's actual call
                VisLang.VisNode? arg = _visNodes.FirstOrDefault(p => p.DebugData == input.Connection.OwningNode);
                if (arg != null)
                {
                    inputs.Add(arg);
                }
                else
                {
                    throw new VisLangVisualParserException("Attempted to use return of executable function before function was called");
                }
                continue;
            }
            // generate the data node tree
            VisLang.VisNode? data = _visNodes.FirstOrDefault(p => p.DebugData == input.Connection.OwningNode) ?? GenerateDataTree(input.Connection.OwningNode);
            if (data != null)
            {
                inputs.Add(data);
            }
        }
        return inputs;
    }

    /// <summary>
    /// This simply traverses the line from root node and till the last available node. This does not generate data nodes
    /// </summary>
    /// <param name="root">Start editor node</param>
    /// <param name="_system">Interpreter system itself</param>
    /// <returns>Resulting root node with child nodes attached or null if generation failed</returns>
    private VisLang.VisNode? GenerateExecutionTree(VisNode root)
    {
        VisLang.ExecutionNode? node = root.CreateNode<VisLang.ExecutionNode>(_system);
        if (node == null)
        {
            return null;
        }
        node.DebugData = node;
        node.Inputs = GenerateInputs(root);

        if (root.OutputExecNode != null && root.OutputExecNode.Connection != null && root.OutputExecNode.Connection.OwningNode != null)
        {
            VisLang.VisNode? next = GenerateExecutionTree(root.OutputExecNode.Connection.OwningNode);
            if (next != null)
            {
                // any node that is connected to exec line is by definition an exec node
                // right????
                // TODO: Actually check if i'm right >_>
                node.DefaultNext = (next as VisLang.ExecutionNode);
            }
        }
        return node;
    }

    private VisLang.DataNode? GenerateDataTree(VisNode root)
    {
        _parsedNodes.Add(root);
        VisLang.DataNode? node = root.CreateNode<VisLang.DataNode>(_system);
        if (node == null)
        {
            return null;
        }
        node.DebugData = root;
        node.Inputs = GenerateInputs(root);
        return node;
    }

    public VisLang.VisNode? Parse()
    {
        return GenerateExecutionTree(_root);
    }

    public NodeParser(VisNode root, VisLang.VisSystem system)
    {
        _system = system;
        _root = root;
    }
}