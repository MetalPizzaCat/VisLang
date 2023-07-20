namespace VisLang.Editor;
using Godot;
using System.Collections.Generic;

/// <summary>
/// Node that represents "If" operation on code<para></para>
/// Due to the fact that it represents not a function but a code structure object it has it's own way of creating the ports
/// </summary>
public partial class EditorGraphBranchNode : EditorGraphNode
{

    public EditorGraphNodeInput? SuccessNextExecutable { get; set; }

    public EditorGraphNodeInput? FailureNextExecutable { get; set; }

    public override void GenerateFunction(FunctionInfo info)
    {
        Title = "Branch";
        // create a dummy function info just so we could mark it as executable and store the node type
        // i could in theory add additional variable for fallback in the parent but this is less hassle
        Info = new FunctionInfo
        (
            "Branch",
            true,
            new() { new("Condition", ValueType.Bool, null) },
            "VisLang.FlowControlIfNode",
            false
        );
        // we have to hijack entire function creation process because If is not a function
        AddChild(new Label() { Text = "Exec Success", HorizontalAlignment = HorizontalAlignment.Center });
        SetSlot(0, true, ExecTypeId, new Color(1, 1, 1), true, ExecTypeId, new Color(1, 1, 1));
        //AddChild(new Label() { Text = "Condition Failure", HorizontalAlignment = HorizontalAlignment.Center });
        CreatePort(new("Condition", ValueType.Bool, null), 1, false, null);
        SetSlot(1, true, GetTypeIdForValueType(ValueType.Bool), CodeTheme?.GetColorForType(ValueType.Bool) ?? new Color(), true, ExecTypeId, new Color(1, 1, 1));

    }

    public override void AddExecConnection(int dstPort, EditorGraphNode node, int srcPort)
    {
        if (dstPort == 0)
        {
            SuccessNextExecutable = new EditorGraphNodeInput(node, srcPort);
        }
        else if (dstPort == 1)
        {
            FailureNextExecutable = new EditorGraphNodeInput(node, srcPort);
        }
    }

    public override bool CanConnectOnPortExec(int port)
    {
        if (port == 0)
        {
            return SuccessNextExecutable == null;
        }
        else if (port == 1)
        {
            return FailureNextExecutable == null;
        }
        return false;
    }

    public override void DestroyConnectionOnPort(int port, bool left)
    {
        // if it's not success and failure ports on the left we should let parent deal with it
        if (left || (port != 1 && port != 0))
        {
            base.DestroyConnectionOnPort(port, left);
            return;
        }
        if (port == 0)
        {
            SuccessNextExecutable = null;
        }
        else if (port == 1)
        {
            FailureNextExecutable = null;
        }
    }
}