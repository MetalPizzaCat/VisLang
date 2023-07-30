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

    public Vector2 CanvasPosition
    {
        get => PositionOffset;
        set => PositionOffset = value;
    }

    /// <summary>
    /// Node that is connected to be executed next
    /// </summary>
    public EditorGraphNodeInput? NextExecutable { get; set; } = null;

    public List<EditorGraphNodeInput?> Inputs { get; set; } = new();
    public List<EditorGraphInputControl> InputControls { get; private set; } = new();
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

    protected int? ArrayTypeListeningPort { get; set; }

    protected void CreatePort(FunctionInputInfo arg, int slotIndex, bool displayOutput = false, VisLang.ValueType? outputType = null)
    {
        EditorGraphInputControl inp = new(arg);
        InputControls.Add(inp);
        AddChild(inp);

        if (displayOutput && outputType != null)
        {
            SetSlot
            (
                slotIndex,
                true,
                arg.TypeMatchingPermissions == FunctionInputInfo.TypePermissions.AllowAny ? AnyTypeId : GetTypeIdForValueType(arg.InputType),
                (arg.TypeMatchingPermissions == FunctionInputInfo.TypePermissions.AllowAny ? CodeTheme?.AnyColor : CodeTheme?.GetColorForType(arg.InputType)) ?? new Color(1, 1, 1),
                true,
                GetTypeIdForValueType(outputType.Value),
                CodeTheme?.GetColorForType(outputType ?? VisLang.ValueType.Bool) ?? new Color(1, 1, 1)
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
    }

    /// <summary>
    /// Generate ports for a given function signature and return last used port
    /// </summary>
    /// <param name="info">Function signature</param>
    /// <returns>Index of the last created port</returns>
    protected int GeneratePorts(FunctionInfo info)
    {
        // we don't quite care what happens to the inputs so we let garbage collector deal with it
        Inputs.Clear();
        // but we have to manually remove input controls because other wise godot's node will keep them alive
        // not using InputControls list for this because exec ports should be deleted as well
        GetChildren().ToList().ForEach(p =>
        {
            RemoveChild(p);
            p.QueueFree();
        });
        InputControls.Clear();

        Title = info.FunctionName;
        TooltipText = info.FunctionDescription;

        int slotIndex = 0;
        if (info.IsExecutable)
        {
            AddChild(new Label() { Text = "Exec", HorizontalAlignment = HorizontalAlignment.Center });
            SetSlot(slotIndex, true, ExecTypeId, new Color(1, 1, 1), true, ExecTypeId, new Color(1, 1, 1));
            slotIndex++;
        }
        // in the original node implementation we created a special input object and listened to it
        // but because all of the connections in this variation are handled inside of the node we don't need to manually create any object
        foreach (FunctionInputInfo arg in info.Inputs)
        {
            CreatePort(arg, slotIndex, slotIndex == (info.IsExecutable ? 1 : 0) && info.HasOutput, info.OutputType);
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
        if (info.IsArrayTypeDependent)
        {
            ArrayTypeListeningPort = info.ArrayTypeDefiningArgumentId + (info.IsExecutable ? 1 : 0);
        }
        return slotIndex;
    }

    /// <summary>
    /// Generates ports and updates connection based on the given function signature
    /// </summary>
    /// <param name="info">Function signature to use as base</param>
    public virtual void GenerateFunction(FunctionInfo info)
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

    /// <summary>
    /// Update types of all ports that represent array type dependent arguments<para></para>
    /// This will cause any inputs that have connections and that will have their connections invalidated to create "Invalid" ports
    /// </summary>
    /// <param name="type">Type to change to, or any if type is null</param>
    protected void UpdateArrayDependentInputs(VisLang.ValueType? type)
    {
        GD.Print($"Changing to {type}");
        if (Info == null)
        {
            GD.PrintErr("Tried to change node inputs to array type but no function was set");
            return;
        }
        // port0 is reserved for executable inputs
        int portId = Info.IsExecutable ? 1 : 0;
        foreach (FunctionInputInfo input in Info.Inputs)
        {
            if (input.IsArrayTypeDependent && portId != ArrayTypeListeningPort.Value)
            {
                int typeId = type == null ? AnyTypeId : GetTypeIdForValueType(type.Value);
                SetSlot
                (
                    portId,
                    IsSlotEnabledLeft(portId),
                    typeId,
                    CodeTheme?.GetColorForType(type) ?? new Color(),
                    IsSlotEnabledRight(portId),
                    GetSlotTypeRight(portId),
                    GetSlotColorRight(portId)
                );
            }
            portId++;
        }
    }

    protected void UpdateArrayDependentOutput(VisLang.ValueType? type)
    {
        if (Info == null)
        {
            GD.PrintErr("Tried to change node inputs to array type but no function was set");
            return;
        }
        if (Info.HasOutput && Info.IsOutputArrayTypeDependent)
        {
            int typeId = type == null ? AnyTypeId : GetTypeIdForValueType(type.Value);
            int portId = Info.IsExecutable ? 1 : 0;
            SetSlot
            (
                portId,
                IsSlotEnabledLeft(portId),
                GetSlotTypeLeft(portId),
                GetSlotColorLeft(portId),
                IsSlotEnabledRight(portId),
                typeId,
                CodeTheme?.GetColorForType(type) ?? new Color()
            );
        }
    }

    /// <summary>
    /// Records a new connection to a node 
    /// </summary>
    /// <param name="dstPort">Port on this node that is being connected to</param>
    /// <param name="node">Node that connection is coming from</param>
    /// <param name="srcPort">Port on the node from which connection is coming</param>
    public void AddConnection(int dstPort, EditorGraphNode node, int srcPort)
    {
        // while ports in godot node count the exec input as port
        // Inputs are only for data connections, so execs port values are shifted by one
        Inputs[IsExecutable ? (dstPort - 1) : dstPort] = new EditorGraphNodeInput(node, srcPort);
        if (ArrayTypeListeningPort != null && ArrayTypeListeningPort.Value == dstPort)
        {
            // a node can have only one data output and if we are connecting it's output to our input then we are connecting the output
            // right?
            UpdateArrayDependentInputs(node.Info?.OutputArrayType);
            UpdateArrayDependentOutput(node.Info?.OutputArrayType);
        }
        else
        {
            InputControls[IsExecutable ? (dstPort - 1) : dstPort].HasManualInput = false;
        }
    }

    /// <summary>
    /// Records a new exec line connection<para></para>
    /// Unlike data connection exec connection is left to right meaning that THIS node is the source node<para></para>
    /// By default dstPort value is ignored because only one exec port can be present on a function node
    /// </summary>
    /// <param name="dstPort">Port on this node that connection is coming from</param>
    /// <param name="node">Node to connect to</param>
    /// <param name="srcPort">Port on the node where line is connecting</param>
    public virtual void AddExecConnection(int dstPort, EditorGraphNode node, int srcPort)
    {
        NextExecutable = new EditorGraphNodeInput(node, srcPort);
    }

    /// <summary>
    /// Destroyed connection on selected port
    /// </summary>
    /// <param name="port">Port id </param>
    /// <param name="left">If true left(input side) connection will be destroyed otherwise right(output side)</param>
    public virtual void DestroyConnectionOnPort(int port, bool left)
    {
        // base executables will always have port0 reserved for exec line
        // check for left because otherwise disconnecting on the left  will disconnect right
        // although it never occurred before the check was added so who knows?
        if (IsExecutable && port == 0 && !left)
        {
            NextExecutable = null;
        }
        // this should only occur when we are trying to detach a connection on the exec right
        // a connection that we don't keep any record about
        else if (IsExecutable && port == 0)
        {
            return;
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
                InputControls[port - (IsExecutable ? 1 : 0)].HasManualInput = true;
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

    public virtual bool CanConnectOnPortExec(int port)
    {
        return NextExecutable == null;
    }
    public virtual bool CanConnectOnPort(int dstPort)
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

    public virtual Files.EditorNodeSaveData GetSaveData()
    {
        return new Files.EditorNodeSaveData(Name, Position, Info?.ResourcePath);
    }

    public void LoadData(Files.EditorNodeSaveData data)
    {
        CanvasPosition = data.Position;
        Name = data.Name;
    }
}
