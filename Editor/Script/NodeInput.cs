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

    public object Data
    {
        get
        {
            switch (_inputType)
            {
                case VisLang.ValueType.Bool:
                    return BoolInput.ButtonPressed;
                case VisLang.ValueType.Number:
					// using float because i said so >:(
                    return (float)NumberInput.Value;
                case VisLang.ValueType.String:
                    return StringInput.Text;
            }
			return 0;
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
