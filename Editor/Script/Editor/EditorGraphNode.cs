namespace VisLang.Editor;

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class EditorGraphNode : GraphNode
{
    [Export]
    public FunctionInfo? Info { get; set; }
    [Export]
    public CodeColorTheme? CodeTheme { get; set; }

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
    }

    public void AddConnection(int dstPort, EditorGraphNode node, int srcPort)
    {
        Inputs[dstPort] = new EditorGraphNodeInput(node, srcPort);
    }

    public bool CanConnectOnPortExec(int port, bool asSource)
    {
        return (asSource ? NextExecutable : PreviousExecutable) == null;
    }
    public bool CanConnectOnPort(int dstPort)
    {
        return Inputs.ElementAtOrDefault(dstPort) == null;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
