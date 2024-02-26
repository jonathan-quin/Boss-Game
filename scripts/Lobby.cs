using Godot;
using System;

public partial class Lobby : Node
{
	[Export]
	public PackedScene SurviorScene;

    public override void _Ready()
    {
        //print("I am " + str(multiplayer.get_unique_id()))
		

		
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
	}


    /**
	Used only by server
	Spawns a player. The MultiplayerSpawner will automatically replicate the instance on all clients.
	**/
    public void CreatePlayer(long id = 1){
		GD.Print("made a player");
	
		Survivor player = SurviorScene.Instantiate() as Survivor;
		player.Name = id.ToString();
		
		player.Position = Vector3.Up * 1.5f;
		
		var objectHolder = GetNode<Node3D>("%PlayerSpawnRoot"); 
		//objectHolder.CallDeferred("AddChild",player);
		objectHolder.AddChild(player);
		

	}
	
	
	

	/**
	Used only by server
	Called when multiplayer detects a client has left.
	removes the client's avatar and then syncs with all clients through multiplayer spawner.
	**/
	public void DeletePlayer(long id = 1){
		if (id != 1){
			GD.Print("Called the RPC");
			Rpc("_DeletePlayer",id);
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	public void _DeletePlayer(long id){

		GD.Print("Received the RPC");

		Node objectHolder = GetNode<Node3D>("%PlayerSpawnRoot");
		objectHolder.GetNode<Node>(id.ToString()).QueueFree();
	}

}

/*
extends Node3D

@export var surviorScene : PackedScene

# Called when the node enters the scene tree for the first time.
func _ready():
	
	print("I am " + str(multiplayer.get_unique_id()))
	
	#all players have these signals connected for debugging
	multiplayer.connected_to_server.connect(connectedToServer)
	multiplayer.connection_failed.connect(failedToConnect)
	
	
	if multiplayer.is_server():
		#the server is set to create new players immediately when they join, but this behavior will be changed later to not interupt games.
		multiplayer.peer_connected.connect(create_player)
		multiplayer.peer_disconnected.connect(del_player)
		create_player()
	else:
		multiplayer.server_disconnected.connect(serverDisconnected)
	




func connectedToServer():
	print("connected! " + str(multiplayer.get_unique_id))
func failedToConnect():
	print("failed. " + str(multiplayer.get_unique_id))
func serverDisconnected():
	Globals.peer.close()
	get_tree().change_scene_to_file("res://scenes/main_menu.tscn")

"""
Used only by server
Spawns a player. The MultiplayerSpawner will automatically replicate the instance on all clients.
"""
func create_player(id = 1):
	
	print("made a player")
	
	var player = surviorScene.instantiate()
	player.name = str(id)
	
	player.position = Vector3.UP
	
	var objectHolder = %PlayerSpawnRoot
	objectHolder.call_deferred("add_child",player)
	
	

"""
Used only by server
Called when multiplayer detects a client has left.
removes the client's avatar and then syncs with all clients through multiplayer spawner.
"""
func del_player(id = 1):
	if id != 1:
		
		rpc("_del_player",id)
		
@rpc("any_peer", "call_local") func _del_player(id):
	var objectHolder = %PlayerSpawnRoot
	objectHolder.get_node(str(id)).queue_free()

##for 
#func remove_player(id):
	#multiplayer.peer_disconnected.connect(del_player)
	#del_player(id)

*/

