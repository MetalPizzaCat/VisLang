namespace VisLang.Editor;
using Godot;
using System.Collections.Generic;

/// <summary>
/// Node that represents "If" operation on code<para></para>
/// Due to the fact that it represents not a function but a code structure object it has it's own way of creating the ports
/// </summary>
public partial class EditorGraphForLoopNode : EditorGraphSplitExecutionNode
{

    public static readonly int BodyExecPortId = 0;
    public static readonly int FinishedExecPortId = 2;

    public string InternalIteratorVariableName { get; }

    public EditorGraphForLoopNode()
    {
        InternalIteratorVariableName = System.Guid.NewGuid().ToString();
    }

    public override void GenerateFunction(FunctionInfo info)
    {
        Title = "For loop";
        // create a dummy function info just so we could mark it as executable and store the node type
        // i could in theory add additional variable for fallback in the parent but this is less hassle
        Info = new FunctionInfo
        (
            "ForLoop",
            true,
            new()
            {
                new("Start", ValueType.Integer, null),
                new("Stop", ValueType.Integer, null),
                new("Step", ValueType.Integer, null)
            },
            "VisLang.ForNode",
            false
        );
        // we have to hijack entire function creation process because If is not a function
        AddChild(new Label() { Text = "Exec Body", HorizontalAlignment = HorizontalAlignment.Center });
        SetSlot(BodyExecPortId, true, ExecTypeId, new Color(1, 1, 1), true, ExecTypeId, new Color(1, 1, 1));

        //AddChild(new Label() { Text = "Condition Failure", HorizontalAlignment = HorizontalAlignment.Center });
        CreatePort(new("Start", ValueType.Integer, null), 1, true, ValueType.Integer);
        CreatePort(new("Stop", ValueType.Integer, null), 2, false, null);
        CreatePort(new("Step", ValueType.Integer, null), 3, false, null);
        SetSlot(FinishedExecPortId, true, GetTypeIdForValueType(ValueType.Integer), CodeTheme?.GetColorForType(ValueType.Integer) ?? new Color(), true, ExecTypeId, new Color(1, 1, 1));

    }

    protected override void ApplyAdditionalCreationData<NodeType>(NodeType node)
    {
        base.ApplyAdditionalCreationData(node);
        if(node is ForNodeBase loop)
        {
            loop.IteratorVariableName = InternalIteratorVariableName;
        }
    }

    public override void AddExecConnection(int dstPort, EditorGraphNode node, int srcPort)
    {
        if (dstPort == BodyExecPortId)
        {
            UpperNextExecutable = new EditorGraphNodeInput(node, srcPort);
        }
        else if (dstPort == FinishedExecPortId)
        {
            LowerNextExecutable = new EditorGraphNodeInput(node, srcPort);
        }
    }

    public override bool CanConnectOnPortExec(int port)
    {
        if (port == BodyExecPortId)
        {
            return UpperNextExecutable == null;
        }
        else if (port == FinishedExecPortId)
        {
            return LowerNextExecutable == null;
        }
        return false;
    }

    public override void DestroyConnectionOnPort(int port, bool left)
    {
        // if it's not success and failure ports on the left we should let parent deal with it
        if (left || (port != FinishedExecPortId && port != BodyExecPortId))
        {
            base.DestroyConnectionOnPort(port, left);
            return;
        }
        if (port == BodyExecPortId)
        {
            UpperNextExecutable = null;
        }
        else if (port == FinishedExecPortId)
        {
            LowerNextExecutable = null;
        }
    }
}