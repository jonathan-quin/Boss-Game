using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class Lobby : Node
{
	[Export]
	public PackedScene SurviorScene;
	[Export]
	public PackedScene BossScene;
	

	public override void _Ready()
	{
		Globals.objectHolder = GetNode<Node3D>("%PlayerSpawnRoot");
		Globals.multiplayerSpawner = GetNode<CustomMultiplayerSpawner>("%MultiplayerSpawner");
	}


	/// <summary>
	/// Uses the game start request to start the correct game. This function calls endgame to clear everything. 
	/// 
	/// This is only called by the lobby interface of the Host. Only the host should call this function.
	/// </summary>
	/// <param name="gameStartRequest"></param>
	
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

		/*GD.Print("Survior:");
		foreach (Vector3 vector in survivorSpawnPositions){
			GD.Print(vector);
		}
		GD.Print("Boss:");
		foreach (Vector3 vector in bossSpawnPositions){
			GD.Print(vector);
		}*/





		switch (gameStartRequest.gameMode)
		{
			case gameStartRequest.GameMode.HOST_BOSS:
				break;

			case gameStartRequest.GameMode.REQUEST_BOSS:

				//GD.Print("requesting boss");

				foreach (PlayerConfiguration config in gameStartRequest.playerConfigurations)
				{
					Vector3 newPos = config.wantsToBeBoss ? PopFront<Vector3>(bossSpawnPositions) : PopFront<Vector3>(survivorSpawnPositions);
					//GD.Print(config.wantsToBeBoss ? "boss" : "survivor", " ", newPos);
					CreatePlayer(config.wantsToBeBoss, newPos, config.id);
				}

				break;
		}

		var gameStartNodes = GetTree().GetNodesInGroup("callOnGameStart");

		//Game objects need to have the game start interface and be in the callOnGameStart group in order to be set off. Using ready instead causes problems.
		foreach (GameStartInterface obj in gameStartNodes){
			obj.start();
		}

	}


	bool displayingMessage = false;
	/// <summary>
	/// Checks every frame the game is running if any category of player is all dead. 
	/// It determines if survivors are dead by whether they have instances or not. This only works because players are deleted when they die.
	/// </summary>
	/// <param name="delta"></param>
	public override async void _Process(double delta)
	{

		if (!Globals.gameInProgress || displayingMessage || !Multiplayer.IsServer()) return; 


		if (Survivor.AllSurvivorsAreDead())
		{
			displayingMessage = true;

			sendMessageToAllClients("Boss has won!");

			//The timer is to wait for the message to go through before ending the game.
			GetTree().CreateTimer(5.0).Timeout += endGame;

			GD.Print("all survivors are dead.");

		}
		if (boss.AllBossesAreDead())
		{
			displayingMessage = true;

			sendMessageToAllClients("Survivors have won!");

			GetTree().CreateTimer(5.0).Timeout += endGame;

			GD.Print("all bosses are dead.");

		}


	}

	/// <summary>
	/// Ends the game. 
	/// Clients don't keep track of whether or not the game is in progress, 
	/// so the clients only know the game is over because their player has been deleted.
	/// 
	/// This also deletes every object in the "deleteOnGameEnd" group. Objects not in this group will persist.
	/// 
	/// This function is called at the beginning of start game to ensure all game end objects are gone.
	/// </summary>
	public void endGame(){
		displayingMessage=false;

		var gameStartNodes = GetTree().GetNodesInGroup("deleteOnGameEnd");

		foreach (Node obj in gameStartNodes){
			obj.QueueFree();
		}

		lobbyInterface.instance.open();

		Globals.gameInProgress = false;

	}

	/// <summary>
	/// Has the instance of the player ui on this client tell every other playui and itself to display a message.
	/// </summary>
	/// <param name="message"></param>
	 public void sendMessageToAllClients(string message)
	{
		PlayerUI playerUi = (PlayerUI)GetTree().GetFirstNodeInGroup("playerUi");

		GD.Print(lobbyInterface.lobbyPlayers);

		foreach (lobbyPlayer lobbyPlayer in lobbyInterface.lobbyPlayers)
		{
			playerUi.displayMessage(message,lobbyPlayer.ID);
		}
	}


	//Helper function. Might be moved to a different class later.
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
		//GD.Print("made a player");
	
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


