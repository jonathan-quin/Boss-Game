extends CharacterBody3D


var SPEED = 5.0
const ACCEL = 6.0
const DEACCEL = 8.0
const JUMP_VELOCITY = 4.5

var freeMouse = true
@export var speedBase = 15
@export var sprintBase = 30

# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity = ProjectSettings.get_setting("physics/3d/default_gravity")

func _enter_tree():
	
	set_multiplayer_authority(name.to_int())
	

func _ready():
	if is_multiplayer_authority():
		%Camera.current = true
	
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED if !freeMouse else Input.MOUSE_MODE_VISIBLE;
	
	if name == "1":
		speedBase = 20
		sprintBase = 40
		scale = Vector3(2, 2, 2)
	

func _physics_process(delta):
	
	if !is_multiplayer_authority():
		return
	
	#toggle mouse being locked
	if Input.is_action_just_pressed("escape"):
		freeMouse = !freeMouse
	elif freeMouse and Input.is_action_just_pressed("leftClick"):
		freeMouse = false
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED if !freeMouse else Input.MOUSE_MODE_VISIBLE;
	
	
	
	move(delta)
	
	
	
	
	

func move(delta):
	if not is_on_floor():
		velocity.y -= gravity * delta
	
	# Handle jump.
	if Input.is_action_just_pressed("jump") and is_on_floor():
		velocity.y = JUMP_VELOCITY
		
	if Input.is_action_pressed("sprint"):
		SPEED = sprintBase
	else:
		SPEED = speedBase
	
	# Get the input direction and handle the movement/deceleration.
	var input_dir = Input.get_vector("left", "right", "forward", "backward")
	#we set the forward direction to where the body is facing.
	var direction = (basis * Vector3(input_dir.x, 0, input_dir.y)).normalized()
	if direction:
		velocity.x = lerp(velocity.x, direction.x * SPEED, ACCEL * delta) 
		velocity.z = lerp(velocity.z, direction.z * SPEED, ACCEL * delta) 
	else:
		velocity.x = lerp(velocity.x, 0.0, DEACCEL * delta)
		velocity.z = lerp(velocity.z, 0.0, DEACCEL * delta)
	
	move_and_slide()
	



const SENSITIVITY = 0.0015

func _input(event):
	
	if !is_multiplayer_authority():
		return
	
	#don't move camera if mouse is captive
	if freeMouse:
		return
	
	pass
	if event is InputEventMouseMotion:
		
		var neck = $neck
		var head = $neck/head
		
		head.rotation.x += (-event.relative.y * SENSITIVITY)
		rotation.y += (-event.relative.x * SENSITIVITY)
		
		
		head.rotation.x = clamp(head.rotation.x, deg_to_rad(-90),deg_to_rad(90))
