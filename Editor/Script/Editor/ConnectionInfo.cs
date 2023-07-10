namespace VisLang.Editor.Parsing;

public record ConnectionInfo(EditorGraphNode Source, int SourcePortId, EditorGraphNode Destination, int DestinationPortId);
