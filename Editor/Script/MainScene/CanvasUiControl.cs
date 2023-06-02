using Godot;
using System;

public partial class CanvasUiControl : Control
{
    [Export]
    public PopupPanel? NodeCreationPopup { get; set; }

    [Export]
    public CanvasControl? CanvasControl { get; set; }

    public bool IsContextMenuClickEnabled { get; set; } = false;

    public override void _GuiInput(InputEvent @event)
    {
        base._GuiInput(@event);
        if (@event is InputEventMouseButton mouse && mouse.Pressed && mouse.ButtonIndex == MouseButton.Right)
        {
            SummonContextMenu();
        }
    }

    // public override void _Input(InputEvent @event)
    // {
    //     base._Input(@event);
    //     if (Input.IsActionJustPressed("context_menu_click") && IsContextMenuClickEnabled)
    //     {
    //         SummonContextMenu();
    //     }
    // }

    private void SummonContextMenu()
    {
        if (NodeCreationPopup != null)
        {
            GD.Print("Show popup");
            NodeCreationPopup.Position = new Vector2I((int)(CanvasControl?.MousePosition.X ?? 0), (int)(CanvasControl?.MousePosition.Y ?? 0));
            NodeCreationPopup.Popup();
        }
    }

    private void EnableContextMenuClick()
    {
        IsContextMenuClickEnabled = true;
    }

    private void DisableContextMenuClick()
    {
        IsContextMenuClickEnabled = false;
    }
}
