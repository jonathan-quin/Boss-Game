using Godot;
using System;

public partial class PauseMenu : Control
{

	[Export]
	public string mainMenuPath;

	public lobbyInterface lobbyInterface;

	public Button resumeGameButton;
	public Button MainMenuButton;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{

		resumeGameButton = GetNode<Button>("%ResumeButton");
		resumeGameButton.Pressed += resumeGame;

		MainMenuButton = GetNode<Button>("%MainMenuButton");
		MainMenuButton.Pressed += goToMenu;
		
	}

	public void resumeGame(){
		lobbyInterface.resume();
	}

	public void goToMenu(){
		Multiplayer.MultiplayerPeer.Close();
		GetTree().Paused = false;
		Input.MouseMode = Input.MouseModeEnum.Visible;
		GetTree().ChangeSceneToFile(mainMenuPath);
	}

}
