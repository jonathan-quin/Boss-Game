using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Lobby : Node
{
	[Export]
	public PackedScene SurviorScene;
	[Export]
	public PackedScene BossScene;

	public override void _Ready()
	{
		Globals.objectHolder = GetNode<Node3D>("%PlayerSpawnRoot");
		Globals.multiplayerSpawner = GetNode<MultiplayerSpawner>("%MultiplayerSpawner");
	}

	public void StartGame(gameStartRequest gameStartRequest)
	{

		endGame();

		Globals.gameInProgress = true;

		var spawnPoints = GetTree().GetNodesInGroup("spawnPoint");

		Random random = new Random();
		List<Vector3> survivorSpawnPositions = spawnPoints
											.Where(s => (s as spawnPoint).spawnType == spawnPoint.SpawnType.SURVIVOR)
											.Select(s => (s as spawnPoint).Position)
											.OrderBy(x => random.Next())
											.ToList();
		List<Vector3> bossSpawnPositions = spawnPoints
											.Where(s => (s as spawnPoint).spawnType == spawnPoint.SpawnType.BOSS)
											.Select(s => (s as spawnPoint).Position)
											.OrderBy(x => random.Next())
											.ToList();



		switch (gameStartRequest.gameMode)
		{
			case gameStartRequest.GameMode.HOST_BOSS:
				break;

			case gameStartRequest.GameMode.REQUEST_BOSS:

				GD.Print("requesting boss");

				foreach (PlayerConfiguration config in gameStartRequest.playerConfigurations)
				{
					GD.Print(config.wantsToBeBoss);
					CreatePlayer(config.wantsToBeBoss, config.wantsToBeBoss ? PopFront<Vector3>(bossSpawnPositions) : PopFront<Vector3>(survivorSpawnPositions), config.id);
				}

				break;
		}

		var gameStartNodes = GetTree().GetNodesInGroup("callOnGameStart");

		foreach (GameStartInterface obj in gameStartNodes){
			obj.start();
		}

	}

	public void endGame(){

		var gameStartNodes = GetTree().GetNodesInGroup("deleteOnGameEnd");

		foreach (Node obj in gameStartNodes){
			obj.QueueFree();
		}

		lobbyInterface.instance.open();

		Globals.gameInProgress = false;

	}

	public static T PopFront<T>(List<T> list)
	{
		if (list.Count == 0)
		{
			throw new InvalidOperationException("List is empty.");
		}

		T poppedElement = list[0];
		list.RemoveAt(0);
		return poppedElement;
	}

	/**
	Used only by server
	Spawns a player. The MultiplayerSpawner will automatically replicate the instance on all clients.
	**/
	public void CreatePlayer(bool isBoss, Vector3 spawnPosition, long id = 1 )
	{
		GD.Print("made a player");
	
		var player = (isBoss ? BossScene : SurviorScene ).Instantiate() as Node3D;
		player.Name = id.ToString();
		
		player.Position = spawnPosition;
		
		Globals.objectHolder.AddChild(player);
	}
	
	
	

	/**
	Used only by server
	Called when multiplayer detects a client has left.
	removes the client's avatar and then syncs with all clients through multiplayer spawner.
	**/
	public void DeletePlayer(long id = 1){
		if (id != 1){
			Globals.objectHolder.GetNode<Node>(id.ToString()).QueueFree();
		}
	}

}


