using Godot;
using System;

public partial class StartButton : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = Multiplayer.IsServer();
	}

	public override void _Pressed()
	{
		base._Pressed();
		Visible = false;
	}
}
