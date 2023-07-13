namespace VisLang.Editor;

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class EditorGraphNode : GraphNode
{
    public delegate void DeleteRequestedEventHandler(EditorGraphNode? sender);
    public event DeleteRequestedEventHandler? DeleteRequested;

    [Export]
    public FunctionInfo? Info { get; set; }
    [Export]
    public CodeColorTheme? CodeTheme { get; set; }

    public bool CanBeDeleted { get; protected set; } = true;

    public bool IsExecutable => Info?.IsExecutable ?? false;

    /// <summary>
    /// Node that is connected to be executed next
    /// </summary>
    public EditorGraphNode? NextExecutable { get; set; } = null;

    /// <summary>
    /// Node that is connected to be executed next
    /// </summary>
    public EditorGraphNode? PreviousExecutable { get; set; } = null;

    public List<EditorGraphNodeInput?> Inputs { get; set; } = new();
    public static readonly int ExecTypeId = 0;
    public static readonly int AnyTypeId = 1;


    public static int GetTypeIdForValueType(VisLang.ValueType type)
    {
        // 0 is for exec
        // 1 is for any
        return (int)type + 2;
    }


    public void GenerateFunction(FunctionInfo info)
    {
        if (info != Info)
        {
            Info = info;
        }
        Inputs.Clear();
        Title = info.FunctionName;
        TooltipText = info.FunctionDescription;
        int slotIndex = 0;
        if (info.IsExecutable)
        {
            AddChild(new Label() { Text = "Exec", HorizontalAlignment = HorizontalAlignment.Center });
            SetSlot(slotIndex, true, ExecTypeId, new Color(1, 1, 1), true, ExecTypeId, new Color(1, 1, 1));
            slotIndex++;
        }
        foreach (FunctionInputInfo arg in info.Inputs)
        {
            AddChild(new Label() { Text = arg.InputName });
            if (slotIndex == 0 && info.HasOutput && info.OutputType != null)
            {
                SetSlot
                (
                    slotIndex,
                    true,
                    arg.TypeMatchingPermissions == FunctionInputInfo.TypePermissions.AllowAny ? AnyTypeId : GetTypeIdForValueType(arg.InputType),
                    (arg.TypeMatchingPermissions == FunctionInputInfo.TypePermissions.AllowAny ? CodeTheme?.AnyColor : CodeTheme?.GetColorForType(arg.InputType)) ?? new Color(1, 1, 1),
                    true,
                    GetTypeIdForValueType(info.OutputType.Value),
                    CodeTheme?.GetColorForType(info.OutputType ?? VisLang.ValueType.Bool) ?? new Color(1, 1, 1)
                );
            }
            else
            {
                SetSlot
                (
                    slotIndex,
                    true,
                    arg.TypeMatchingPermissions == FunctionInputInfo.TypePermissions.AllowAny ? AnyTypeId : GetTypeIdForValueType(arg.InputType),
                    (arg.TypeMatchingPermissions == FunctionInputInfo.TypePermissions.AllowAny ? CodeTheme?.AnyColor : CodeTheme?.GetColorForType(arg.InputType)) ?? new Color(1, 1, 1),
                    false,
                    0,
                    new Color()
                );
            }
            Inputs.Add(null);
            slotIndex++;
        }
        // if we have no arguments the output generation will be skipped because it uses arg0 for position
        if (info.Inputs.Count == 0 && info.HasOutput && info.OutputType != null)
        {
            AddChild(new Label() { Text = "Value" });
            SetSlot
                (
                    0,
                    false,
                    0,
                    new Color(),
                    true,
                    GetTypeIdForValueType(info.OutputType.Value),
                    CodeTheme?.GetColorForType(info.OutputType ?? VisLang.ValueType.Bool) ?? new Color(1, 1, 1)
                );
        }
    }

    public override void _Ready()
    {
        if (Info != null)
        {
            GenerateFunction(Info);
        }
        CustomMinimumSize = new Vector2(200, 50);
        ShowClose = CanBeDeleted;
        if (CanBeDeleted)
        {
            CloseRequest += () => DeleteRequested?.Invoke(this);
        }
    }

    public void AddConnection(int dstPort, EditorGraphNode node, int srcPort)
    {
        // while ports in godot node count the exec input as port
        // Inputs are only for data connections, so execs port values are shifted by one
        Inputs[!IsExecutable ? dstPort : (dstPort - 1)] = new EditorGraphNodeInput(node, srcPort);
    }

    /// <summary>
    /// Destroyed connection on selected port
    /// </summary>
    /// <param name="port">Port id </param>
    /// <param name="left">If true left(input side) connection will be destroyed otherwise right(output side)</param>
    public virtual void DestroyConnectionOnPort(int port, bool left)
    {
        // base executables will always have port0 reserved for exec line
        if (IsExecutable && port == 0)
        {
            if (left)
            {
                PreviousExecutable = null;
            }
            else
            {
                NextExecutable = null;
            }
        }
        // we don't care about what's on the right since data connections work right to left
        // as opposed to left to right of the exec connections
        else if (left)
        {
            Inputs[port] = null;
        }
    }

    public bool CanConnectOnPortExec(int port, bool asSource)
    {
        return (asSource ? NextExecutable : PreviousExecutable) == null;
    }
    public bool CanConnectOnPort(int dstPort)
    {
        return Inputs.ElementAtOrDefault(dstPort) == null;
    }

    /// <summary>
    /// Used to apply additional data to the VisNode during node creation to allow for more flexible node creation.<para></para>
    /// For example Variable nodes should use this function to write variable name
    /// </summary>
    /// <param name="node">nNode to add data to</param>
    /// <typeparam name="NodeType">Node type</typeparam>
    protected virtual void ApplyAdditionalCreationData<NodeType>(NodeType node) where NodeType : VisLang.VisNode
    {

    }

    /// <summary>
    /// Create executable node based on the function info<para></para>
    /// Due to how nodes are created using reflection, created node must have an empty constructor available
    /// </summary>
    /// <typeparam name="NodeType"></typeparam>
    /// <returns>Node or null if for any reason creation failed</returns>
    public NodeType? CreateInterpretableNode<NodeType>() where NodeType : VisLang.VisNode
    {
        if (Info == null)
        {
            throw new MissingFunctionInfoException("Attempted to create a function but FunctionInfo is null");
        }
        NodeType? node = (NodeType?)Activator.CreateInstance("VisLang", Info.NodeType)?.Unwrap();
        if(node == null)
        {
            return null;
        }
        ApplyAdditionalCreationData(node);
        return node;
    }
}
