using Godot;
using System;

public partial class FloorItem : Node
{

	[Export]
	public PackedScene heldForm;

	private dynamic Globals;
	public override void _EnterTree()
	{
		Globals = GetNode<Node>("/root/Globals");

		
		SetMultiplayerAuthority(Globals.serverHostID);
		
		
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("hey");
		GD.Print("gosh");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void giveToPlayer(Survivor player){

	}



}
