//#define STATIC_CANVAS
using Godot;
using System;

public partial class CanvasControl : Control
{
    [Export]
    public Label? DebugInfoLabel { get; set; }

    private Vector2 _mousePosition = Vector2.Zero;
    public Vector2 MousePosition => _mousePosition;

    private Vector2 _defaultViewportSize = new Vector2(ProjectSettings.GetSetting("display/window/size/viewport_width").AsSingle(), ProjectSettings.GetSetting("display/window/size/viewport_height").AsSingle());
	public Vector2 DefaultViewportSize => _defaultViewportSize;
	
    public override void _Input(InputEvent @event)
    {
        base._Input(@event);
        if (@event is InputEventMouseMotion mouseMotion)
        {
#if !STATIC_CANVAS
            _mousePosition = (mouseMotion.Position - (GetViewportRect().Size - _defaultViewportSize) / 2f - Size / 2f);
            if (DebugInfoLabel != null)
            {
                DebugInfoLabel.Text = $"MousePos: {mouseMotion.Position}, scale: {GetViewportRect().Size - _defaultViewportSize}";
            }
#else
            _mousePosition = mouseMotion.Position;
#endif
        }
    }
}
