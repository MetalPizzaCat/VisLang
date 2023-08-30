using System;
using Godot;
using VisLang.Editor.CallableEditor;

public partial class FunctionManagementSelectionButton : Button
{
    public delegate void FunctionSelectedEventHandler(object sender, CallableFunctionInfo info);
    public event FunctionSelectedEventHandler? FunctionSelected;

    private CallableFunctionInfo _info;

    public FunctionManagementSelectionButton(CallableFunctionInfo info)
    {
        Text = info.Info.FunctionName;
        _info = info;
    }

    public FunctionManagementSelectionButton(string name)
    {

        Text = name;
        _info = new CallableFunctionInfo();
    }

    public override void _Pressed()
    {
        base._Pressed();
        FunctionSelected?.Invoke(this, _info);
    }
}