using System.Collections.Generic;

namespace VisLang.Editor.Debug;

public class CodeExecutionData
{
    public VisSystem System { get; set; }

    public CodeExecutionData(VisSystem system, List<EditorGraphNode> breakpoints)
    {
        System = system;
        BreakpointNodes = breakpoints;
    }

    public List<EditorGraphNode> BreakpointNodes { get; set; }
}