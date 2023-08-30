using Godot;
using System;
using VisLang.Editor.CallableEditor;
using System.Collections.Generic;
using VisLang.Editor;

/// <summary>
/// Object that handles storing the information about user created functions
/// </summary>
public partial class FunctionManagementControl : Control
{
	[Export]
	public VBoxContainer? Container { get; private set; }

	[Export]
	public TabManager? TabManager { get; private set; }

	public List<CallableFunctionInfo> CallableFunctions { get; private set; } = new();

	/// <summary>
	/// Summons controls for creating new function
	/// </summary>
	private void AddNewFunction()
	{
		Container?.AddChild(new FunctionManagementSelectionButton("Function"));
		CallableFunctions.Add(new CallableFunctionInfo());
		TabManager?.OpenNewTab();
	}

	private void OpenFunction()
	{

	}
}
