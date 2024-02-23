extends Node3D

@export var surviorScene : PackedScene

# Called when the node enters the scene tree for the first time.
func _ready():
	
	print("I am " + str(multiplayer.get_unique_id()))
	
	multiplayer.connected_to_server.connect(connectedToServer)
	multiplayer.connection_failed.connect(failedToConnect)
	
	if multiplayer.is_server():
		multiplayer.peer_connected.connect(create_player)
		multiplayer.peer_disconnected.connect(del_player)
		create_player()
	else:
		multiplayer.server_disconnected.connect(serverDisconnected)
	
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func connectedToServer():
	print("connected! " + str(multiplayer.get_unique_id))
func failedToConnect():
	print("failed. " + str(multiplayer.get_unique_id))
func serverDisconnected():
	Globals.peer.close()
	get_tree().change_scene_to_file("res://scenes/main_menu.tscn")

func create_player(id = 1):
	
	print("made a player")
	
	var player = surviorScene.instantiate()
	player.name = str(id)
	
	player.position = Vector3.UP * 4
	
	var objectHolder = %PlayerSpawnRoot
	objectHolder.call_deferred("add_child",player)
	
	

#for server to use to react to players disconnecting
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


