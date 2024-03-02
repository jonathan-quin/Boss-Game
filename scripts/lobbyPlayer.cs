using Godot;
using System;

public partial class lobbyPlayer : Panel
{

	public int ID = 1;
	public bool isBoss = false;

	public override void _EnterTree(){
		SetMultiplayerAuthority(int.Parse(Name));
	}


	Label idLabel;
	LineEdit nameBox;

	CheckBox checkBox;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		idLabel = GetNode<Label>("%Label");
		nameBox = GetNode<LineEdit>("%LineEdit");
		checkBox = GetNode<CheckBox>("%CheckBox");

		idLabel.Text = "ID: " + Multiplayer.GetUniqueId().ToString();
		
		ID = Multiplayer.GetUniqueId();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

		Constants.nameTag = nameBox.Text;
		Constants.isBoss = checkBox.ButtonPressed;



	}


}
