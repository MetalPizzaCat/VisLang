namespace VisLang.Editor.Files;
using Godot;
using System.Collections.Generic;

public class NodeCollectionSaveData
{

    public record StoredConnectionInfo(string Source, int SourcePort, string Destination, int DestinationPort);
    public EditorNodeSaveData? ExecStartData { get; set; }
    public List<StoredConnectionInfo> Connections { get; set; } = new();
    public Vector2 ScrollOffset { get; set; } = Vector2.Zero;
    public List<EditorNodeSaveData> GenericNodes { get; set; } = new();
    public List<VariableNodeSaveData> VariableNodes { get; set; } = new();
    public List<EditorNodeSaveData> BranchNodes { get; set; } = new();
}