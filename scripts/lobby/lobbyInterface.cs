using Godot;
using System;
using System.Collections.Generic;

public partial class lobbyInterface : Control
{
	[Export]
	public PackedScene lobbyPlayerScene;

	public Container playerContainer;
    public Button startGameButton;

    public Lobby lobby;

	public bool gameInProgress = false;

	public PauseMenu pauseMenu;

	public override void _Ready()
	{
        lobby = GetParent() as Lobby;
		playerContainer = GetNode<Container>("%playerVboxContainer");

		pauseMenu = GetNode<PauseMenu>("%pauseMenu");
		pauseMenu.lobbyInterface = this;

        startGameButton = GetNode<Button>("%startGameButton");
		startGameButton.Pressed += StartGame;

        //all players have these signals connected for debugging
        Multiplayer.ConnectedToServer += ConnectedToServer;
		Multiplayer.ConnectionFailed += FailedToConnect;
	
		if (Multiplayer.IsServer()){
			//the server is set to create new players immediately when they join, but this behavior will be changed later to not interupt games.
			Multiplayer.PeerConnected += CreatePlayer;
			Multiplayer.PeerDisconnected += DeletePlayer;

			CreatePlayer();
			}
		else{
			Multiplayer.ServerDisconnected += ServerDisconnected;
			startGameButton.Disabled = true;
		}
	}

	public void ConnectedToServer(){
		GD.Print("Connected! " + Multiplayer.GetUniqueId());
	}
	public void FailedToConnect(){
		GD.Print("Failed to connect. " + Multiplayer.GetUniqueId());
	}

	public void ServerDisconnected(){
		//don't know what to put here for now
		pauseMenu.main

	}

	public List<lobbyPlayer> lobbyPlayers = new List<lobbyPlayer>();

	public void StartGame()
	{
		List<PlayerConfiguration> configList = new List<PlayerConfiguration>();

		foreach (lobbyPlayer player in lobbyPlayers)
		{
			configList.Add(player.getPlayerConfig());
		}

		gameStartRequest startRequest = new gameStartRequest(configList,gameStartRequest.GameMode.REQUEST_BOSS);


        lobby.StartGame(startRequest);

	}

	/**
	Used only by server
	Spawns a player. The MultiplayerSpawner will automatically replicate the instance on all clients.
	**/
	public void CreatePlayer(long id = 1){
		GD.Print("made a player");
	
		var player = lobbyPlayerScene.Instantiate() as lobbyPlayer;// as Survivor;
		player.Name = id.ToString();
		playerContainer.AddChild(player);

		lobbyPlayers.Add(player);


    }
	

	/**
	Used only by server
	Called when multiplayer detects a client has left.
	removes the client's avatar and then syncs with all clients through multiplayer spawner.
	**/
	public void DeletePlayer(long id = 1){
		if (id != 1){
			lobbyPlayer player = playerContainer.GetNode<Node>(id.ToString()) as lobbyPlayer;
            lobbyPlayers.Remove(player);
            player.QueueFree();
            

			lobby.DeletePlayer(id);
        }
	}

	/// <summary>
	/// Used for swapping between the paused state and the captured mouse state.
	/// 
	/// 
	/// </summary>
	public void HandleMouseModeInputs(){
		if (Input.IsActionJustPressed("pause")){
			Globals.freeMouse = !Globals.freeMouse;

			if (Globals.freeMouse){
				Visible = true;
			} else{
				Visible = false;
			}

			Input.MouseMode = Globals.freeMouse ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured; 
		}
	}

	public void resume(){
		Globals.freeMouse = false;

		Visible = false;
			
		Input.MouseMode = Globals.freeMouse ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured; 
	}
	
	public void open(){
		Globals.freeMouse = true;

		Visible = true;
			
		Input.MouseMode = Globals.freeMouse ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured; 
	}



}


