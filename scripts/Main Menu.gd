extends Control

var port = 8910

@export var gameScene : PackedScene

# Called when the node enters the scene tree for the first time.
func _ready():
	
	Input.mouse_mode = Input.MOUSE_MODE_VISIBLE
	
	%port.value = port
	
	#try to guess our own IP. 
	match OS.get_name():
		"Windows":
			%address.text = IP.resolve_hostname(str(OS.get_environment("COMPUTERNAME")),1)
		"macOS":
			%address.text = "10.135.16.136"
			
	
	multiplayer.connected_to_server.connect(connectedToServer)
	multiplayer.connection_failed.connect(failedToConnect)
	
	
	
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	
	port = %port.value
	
	pass


func _on_join_button_pressed():
	
	Globals.peer = ENetMultiplayerPeer.new()
	
	var err = Globals.peer.create_client(%address.text,port)
	multiplayer.set_multiplayer_peer(Globals.peer)
	
	print("client error ", err)
	
	%Message.text = "Awaiting connection on port " + str(port) + " at " + %address.text
	
	print("tried to join")
	
	pass # Replace with function body.

func connectedToServer():
	print("connected as client")
	Globals.multiplayerId = multiplayer.get_unique_id()
	get_tree().change_scene_to_packed(gameScene)

func failedToConnect():
	print("server does not exist")

func _on_quit_button_pressed():
	get_tree().quit()
	pass # Replace with function body.


func _on_host_button_pressed():
	Globals.peer = ENetMultiplayerPeer.new()
	
	var error = Globals.peer.create_server(port,16)
	
	print("host error ",error)
	
	multiplayer.set_multiplayer_peer(Globals.peer)
	
	get_tree().change_scene_to_packed(gameScene)
