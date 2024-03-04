using Godot;
using System;

public partial class lobbyInterface : Node
{
	[Export]
	public PackedScene lobbyPlayerScene;

	[Export]
	public string mainMenuPath;

	public Container playerContainer;

	public Lobby lobby;

	public override void _Ready()
	{
        lobby = GetParent() as Lobby;
		playerContainer = GetNode<Container>("%playerVboxContainer");


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
		Multiplayer.MultiplayerPeer.Close();
		GetTree().ChangeSceneToFile("mainMenuPath");
	}


	/**
	Used only by server
	Spawns a player. The MultiplayerSpawner will automatically replicate the instance on all clients.
	**/
	public void CreatePlayer(long id = 1){
		GD.Print("made a player");
	
		var player = lobbyPlayerScene.Instantiate() as Node3D;// as Survivor;
		player.Name = id.ToString();
		playerContainer.AddChild(player);
	}
	

	/**
	Used only by server
	Called when multiplayer detects a client has left.
	removes the client's avatar and then syncs with all clients through multiplayer spawner.
	**/
	public void DeletePlayer(long id = 1){
		if (id != 1){
			playerContainer.GetNode<Node>(id.ToString()).QueueFree();
		}
	}

}


