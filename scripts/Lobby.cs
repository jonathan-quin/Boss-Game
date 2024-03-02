using Godot;
using System;

public partial class Lobby : Node
{
	[Export]
	public PackedScene SurviorScene;

	public override void _Ready()
	{
        
	}


	/**
	Used only by server
	Spawns a player. The MultiplayerSpawner will automatically replicate the instance on all clients.
	**/
	public void CreatePlayer(long id = 1){
		GD.Print("made a player");
	
		var player = SurviorScene.Instantiate() as Node3D;// as Survivor;
		player.Name = id.ToString();
		
		player.Position = Vector3.Up * 1.5f;
		
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


