using System.Threading;
using System.Threading.Tasks;

namespace VisLang.Editor.Debug;

/// <summary>
/// Class that handles running the code execution process
/// </summary>
public class CodeExecutor
{
    public delegate void BreakpointHitEventHandler(EditorGraphNode hitGraphNode, ExecutionNode hitNode);
    public delegate void ExecutionOverEventHandler();

    public event ExecutionOverEventHandler? ExecutionOver;
    public event BreakpointHitEventHandler? BreakpointHit;

    public CodeExecutionData CodeExecutionData { get; set; }


    public bool IsRunning { get; private set; }

    public bool IsPaused { get; private set; }

    private Thread? _codeLoopThread;
    /// <summary>
    /// This is the reference to the last hit breakpoint node which should be used to avoid getting stuck on a breakpoint forever
    /// </summary>
    private EditorGraphNode? _lastHitBreakpoint = null;

    public ExecutionNode? Current => CodeExecutionData.System.Current;
    public CodeExecutor(CodeExecutionData codeExecutionData)
    {
        CodeExecutionData = codeExecutionData;
        IsPaused = false;
    }

    private void Execute()
    {
        if (Current?.DebugData is EditorGraphNode editorNode && CodeExecutionData.BreakpointNodes.Contains(editorNode) && _lastHitBreakpoint != editorNode)
        {
            _lastHitBreakpoint = editorNode;
            BreakpointHit?.Invoke(editorNode, Current);
            IsPaused = true;
            return;
        }
        // no breakpoint hit so reference is null
        _lastHitBreakpoint = null;
        CodeExecutionData.System.ExecuteNext(null);
    }

    private void Loop()
    {
        while (Current != null && IsRunning && !IsPaused)
        {
            Execute();
        }
        if (Current == null)
        {
            IsRunning = false;
            ExecutionOver?.Invoke();
        }
    }
    /// <summary>
    /// Runs code normally checking for breakpoints on every execution
    /// </summary>
    public void Run()
    {
        IsRunning = true;
        CodeExecutionData.System.Current = CodeExecutionData.System.Entrance;
        CodeExecutionData.System.ExecuteNext(null);

        _codeLoopThread = new Thread(new ThreadStart(Loop));
        _codeLoopThread.Start();
    }

    /// <summary>
    /// Runs the code normally but does not stop on breakpoint
    /// </summary>
    public void RunDisconnected()
    {
        CodeExecutionData.System.Execute();
    }

    public void Resume()
    {
        if (IsRunning && IsPaused)
        {
            IsPaused = false;
            _codeLoopThread = new Thread(new ThreadStart(Loop));
            _codeLoopThread.Start();
        }
    }
}