namespace VisLang.Editor;

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using VisLang.Editor.Parsing;

public partial class EditorGraphNode : GraphNode
{
    public class InvalidConnectionInfo
    {
        public InvalidConnectionInfo(int? currentPortId, int originalPortId, string portName)
        {
            CurrentPortId = currentPortId;
            OriginalPortId = originalPortId;
            PortName = portName;
        }

        public int? CurrentPortId { get; set; }
        public int OriginalPortId { get; set; }
        public string PortName { get; set; }
    }

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

    /// <summary>
    /// Canvas that this node belongs to
    /// </summary>
    /// <value></value>
    public NodeEditCanvas? ParentCanvas { get; set; } = null;


    public static int GetTypeIdForValueType(VisLang.ValueType type)
    {
        // 0 is for exec
        // 1 is for any
        return (int)type + 2;
    }

    /// <summary>
    /// Input ports that should be removed once disconnected
    /// </summary>
    private List<InvalidConnectionInfo> _invalidInputs = new();

    private int? _invalidOutputPortId = null;
    private bool HasInvalidOutput => _invalidOutputPortId != null;

    /// <summary>
    /// Generate ports for a given function signature and return last used port
    /// </summary>
    /// <param name="info">Function signature</param>
    /// <returns>Index of the last created port</returns>
    private int GeneratePorts(FunctionInfo info)
    {
        List<Node> kids = GetChildren().ToList();
        kids.ForEach(p => RemoveChild(p));
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
            slotIndex++;
        }

        return slotIndex;
    }

    /// <summary>
    /// Generates ports and updates connection based on the given function signature
    /// </summary>
    /// <param name="info">Function signature to use as base</param>
    public void GenerateFunction(FunctionInfo info)
    {
        // get list of all existing connections for safe keeping
        // once we delete all previous connections we will have to move them to new ports
        List<ConnectionInfo> oldConnections = ParentCanvas?.GetNodeConnections().Where(p => p.Destination == this || p.Source == this)?.ToList() ?? new();
        // we keep old names because that makes it easier for use to know what went where
        Dictionary<int, string> oldInputNames = new();
        foreach (ConnectionInfo input in oldConnections.Where(p => p.Destination == this))
        {
            // this will only grab the valid connections, but could cause an issue with invalid
            // but since invalid are invalid we just ignore them
            // the -1 if executable comes from Exec using port 0 while not being a part of function inputs
            if (info.Inputs.ElementAtOrDefault(input.DestinationPortId - (IsExecutable ? 1 : 0)) is FunctionInputInfo inp)
            {
                oldInputNames.Add(input.DestinationPortId, inp.InputName);
                _invalidInputs.Add(new InvalidConnectionInfo(null, input.DestinationPortId, inp.InputName));
            }
            else if (_invalidInputs.FirstOrDefault(p => p.CurrentPortId != null && p.CurrentPortId.Value == input.DestinationPortId) is InvalidConnectionInfo conn)
            {
                oldInputNames.Add(input.DestinationPortId, conn.PortName);
            }
        }
        bool anyOutputs = oldConnections.Any(p => p.Source == this);

        if (info != Info)
        {
            Info = info;
        }

        int slotIndex = GeneratePorts(info);

        foreach (ConnectionInfo conn in oldConnections)
        {
            if (conn.Destination == this && oldInputNames.ContainsKey(conn.DestinationPortId))
            {
                AddChild(new Label() { Text = $"INVALID({oldInputNames[conn.DestinationPortId]})" });
                SetSlot(slotIndex, conn.Destination == this, 999, new Color(1f, 0, 0, 1), false, 999, new Color(1f, 0, 0, 1));
                ParentCanvas?.ChangePortsForInputConnection(conn.Source, conn.SourcePortId, conn.Destination, conn.DestinationPortId, slotIndex);
                if (_invalidInputs.FirstOrDefault(p => p.OriginalPortId == conn.DestinationPortId && p.CurrentPortId == null) is InvalidConnectionInfo invalidInfo)
                {
                    invalidInfo.CurrentPortId = slotIndex;
                }
                slotIndex++;
            }
        }
        if (anyOutputs)
        {
            AddChild(new Label() { Text = $"INVALID(Output)" });
            SetSlot(slotIndex, false, 0, new Color(), true, 998, new Color(1f, 0, 0, 1));
            foreach (ConnectionInfo outputs in oldConnections.Where(p => p.Source == this))
            {
                ParentCanvas?.ChangePortsForOutputConnection(this, 0, slotIndex, outputs.Destination, outputs.DestinationPortId);
            }
            _invalidOutputPortId = slotIndex;

            slotIndex++;
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
            if (_invalidInputs.FirstOrDefault(p => p.CurrentPortId != null && p.CurrentPortId.Value == port) is InvalidConnectionInfo info)
            {
                SetSlot(port, false, 0, new Color(), false, 0, new Color());
                foreach (InvalidConnectionInfo inp in _invalidInputs)
                {
                    if (inp.CurrentPortId > port)
                    {
                        inp.CurrentPortId--;
                    }
                }
                RemoveChild(GetChild(port));
                _invalidInputs.Remove(info);
            }
            else
            {
                Inputs[port - (IsExecutable ? 1 : 0)] = null;
            }
        }
        // we now care about what's on the right incase there is an invalid connection
        else if (!left && HasInvalidOutput && (ParentCanvas?.GetNodeConnections().Any(p => p.Source == this && p.SourcePortId == _invalidOutputPortId.Value) ?? false))
        {
            SetSlot(port, false, 0, new Color(), false, 0, new Color());
            RemoveChild(GetChild(port));
            _invalidOutputPortId = null;
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
        if (node == null)
        {
            return null;
        }
        ApplyAdditionalCreationData(node);
        return node;
    }
}
