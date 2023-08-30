using Godot;
using GodotPlugins.Game;
using System.Collections.Generic;

namespace VisLang.Editor;

public partial class TabManager : Control
{
    [Export]
    public TabBar TabBar { get; private set; }

    /// <summary>
    /// Control to which all created Function Edit Controls will be added as a child too
    /// </summary>
    /// <value></value>
    [Export]
    public Control TabContentParentControl { get; set; }

    /// <summary>
    ///  Control for editing the main function of the project
    /// it will also be used for calculating positions of any other added tab
    /// </summary>
    /// <value></value>
    [Export]
    public FunctionEditControl MainFunctionEditor { get; private set; }

    [Export]
    public PackedScene? FunctionEditorPrefab { get; private set; }

    public long ActiveTabId { get; private set; } = 0;

    /// <summary>
    /// List that stores all of the information about all of the currently opened function editors<para/>
    /// This excludes main function as it has it's own window that can not be closed
    /// </summary>
    public List<FunctionEditControl> FunctionEditControls { get; private set; } = new();

    public override void _Ready()
    {
        base._Ready();
        TabBar.TabButtonPressed += CloseTabById;
        TabBar.ActiveTabRearranged += MoveActiveTabToNewId;
        TabBar.TabSelected += SwitchTo;
    }

    public void CloseTabById(long id)
    {

    }

    public void MoveActiveTabToNewId(long newId)
    {

    }

    public void SwitchTo(long id)
    {
        if (ActiveTabId > 0)
        {
            FunctionEditControls[(int)(ActiveTabId - 1)].Visible = false;
        }
        MainFunctionEditor.Visible = id == 0;
        ActiveTabId = id;
        if ((id - 1) >= FunctionEditControls.Count || id < 1)
        {
            return;
        }

        FunctionEditControls[(int)(id - 1)].Visible = true;
        
    }

    public void OpenNewTab()
    {

        FunctionEditControl? control = FunctionEditorPrefab?.InstantiateOrNull<FunctionEditControl>();
        if (control == null)
        {
            GD.PrintErr("Unable to create new function editor because no prefab is present");
            return;
        }
        TabContentParentControl.AddChild(control);
        FunctionEditControls.Add(control);

        TabBar.AddTab("Function");
        TabBar.CurrentTab = TabBar.TabCount - 1;
    }
}