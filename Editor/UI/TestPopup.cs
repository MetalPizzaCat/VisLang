using Godot;
using System;

public partial class TestPopup : Control
{

	public override void _Ready()
	{
		GetNode<PopupPanel>("PopupPanel").Popup();
	}

	private void OnButtonPressed()
	{
		GetNode<PopupPanel>("PopupPanel").Popup();
	}
}
