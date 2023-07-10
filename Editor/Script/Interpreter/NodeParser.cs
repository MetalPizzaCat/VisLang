using System;
using Godot;
using System.Collections.Generic;
using System.Linq;

public class NodeParser
{
    private List<EditorVisNode> _parsedNodes = new();

    /// <summary>
    /// List of all nodes created during code parsing. This will not be used during execution and is meant to be used to allow referring to existing nodes
    /// </summary>
    /// <returns></returns>
    private List<VisLang.VisNode> _visNodes = new();

    private EditorVisNode _root;

    private VisLang.VisSystem _system;

    private List<VisLang.VisNode> GenerateInputs(EditorVisNode root)
    {
        List<VisLang.VisNode> inputs = new();
        foreach (NodeInput input in root.Inputs)
        {
            if (input.Connection == null || input.Connection.OwningNode == null)
            {
                inputs.Add(new VisLang.VariableGetConstNode() { Value = new VisLang.Value(new VisLang.ValueTypeData(input.InputType), input.Data) });
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
    private VisLang.VisNode? GenerateExecutionTree(EditorVisNode root)
    {
        VisLang.ExecutionNode? node = root.CreateNode<VisLang.ExecutionNode>(_system);
        if (node == null)
        {
            return null;
        }
        node.DebugData = root;

        //TODO: Fix
        //node.Inputs = GenerateInputs(root);
        // we record node at this moment because as we will traverse the tree further we need this node to exist to be able to reference it
        _visNodes.Add(node);
        if (root.OutputExecNode != null && root.OutputExecNode.Connection != null && root.OutputExecNode.Connection.OwningNode != null)
        {
            VisLang.VisNode? next = GenerateExecutionTree(root.OutputExecNode.Connection.OwningNode);
            if (next is VisLang.ExecutionNode exec)
            {
                node.DefaultNext = exec;
            }
            else
            {
                throw new VisLangVisualParserException("Attempted to parse the execution tree but found node with invalid connection");
            }
        }

        return node;
    }

    private VisLang.DataNode? GenerateDataTree(EditorVisNode root)
    {
        _parsedNodes.Add(root);
        VisLang.DataNode? node = root.CreateNode<VisLang.DataNode>(_system);
        if (node == null)
        {
            return null;
        }
        node.DebugData = root;
        // TODO: fix
        //node.Inputs = GenerateInputs(root);
        // record this node so any other code that references this node in editor
        // will use it instead of duplicating the data tree
        _visNodes.Add(node);
        return node;
    }

    public VisLang.VisNode? Parse()
    {
        return GenerateExecutionTree(_root);
    }

    public NodeParser(EditorVisNode root, VisLang.VisSystem system)
    {
        _system = system;
        _root = root;
    }
}