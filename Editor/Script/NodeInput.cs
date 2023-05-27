using Godot;
using System;

public partial class NodeInput : Node2D
{

	[Export]
	public Label InputNameLabel { get; set; }

	[Export]
	public LineEdit StringInput { get; set; }
	[Export]
	public SpinBox NumberInput { get; set; }
	[Export]
	public CheckBox BoolInput { get; set; }


	public string InputName
	{
		get => InputNameLabel.Text;
		set => InputNameLabel.Text = value;
	}

	private VisLang.ValueType _inputType = VisLang.ValueType.Bool;

	public VisLang.ValueType InputType
	{
		get => _inputType;
		set
		{
			StringInput.Visible = value == VisLang.ValueType.String;
			NumberInput.Visible = value == VisLang.ValueType.Number;
			BoolInput.Visible = value == VisLang.ValueType.Bool;
			_inputType = value;
		}
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
