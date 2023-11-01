using Godot;
using System;
using System.Linq;
using VisLang.Editor;

namespace VisLang.Editor;
public partial class CustomFunctionControl : Control
{
	[Export]
	public VariableManager VariableManager { get; set; }
	private EditorGraphFunctionDeclarationNode? _currentFunction = null;
	public EditorGraphFunctionDeclarationNode? CurrentFunction
	{
		get => _currentFunction;
		set
		{
			_currentFunction = value;
			if (value == null)
			{
				return;
			}
			VariableManager.Clear();
			foreach (VariableInitInfo variable in value.Variables)
			{
				VariableManager.CreateVariableFromInfo(variable);
			}
		}
	}

	public override void _Ready()
	{
		base._Ready();
		//VariableManager.GetterRequested += SpawnGetter;
		//VariableManager.SetterRequested += SpawnSetter;

		//NodeCanvas.NodeDeleted += RemoveVariableNode;

		VariableManager.VariableNameChanged += UpdateVariableName;
		VariableManager.VariableTypeChanged += UpdateVariableTypeInfo;
		VariableManager.VariableCreated += AddVariable;
	}

	private void UpdateVariableTypeInfo(VariableInfo info, ValueTypeData type)
	{
		if (CurrentFunction?.Variables.FirstOrDefault(p => p.Id == info.Id.ToString()) is VariableInitInfo variable)
		{
			variable.Type = info.FullType;
		}
	}

	private void UpdateVariableName(VisLang.Editor.VariableInfo info, string name)
	{
		if (CurrentFunction?.Variables.FirstOrDefault(p => p.Id == info.Id.ToString()) is VariableInitInfo variable)
		{
			variable.Name = info.Name;
		}
	}

	private void AddVariable(VariableInfo info)
	{
		CurrentFunction?.Variables.Add(new VariableInitInfo(info.Id.ToString(), info.Name, info.FullType));
	}
}
