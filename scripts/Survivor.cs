using Godot;
using System;
using System.Collections.Generic;

public partial class Survivor : CharacterBody3D
{

	public static Dictionary<long,Survivor> survivors = new Dictionary<long,Survivor>();

	const float SPEED = 5.0f;
	const float ACCEL = 6.0f;
	const float DEACCEL = 8.0f;
	const float JUMP_VELOCITY = 4.5f;

	const float GRAVITY = 10f;

	bool freeMouse = true;

	public ItemHolder itemHolder;
	ItemPickupCast itemPickupCast;

	public override void _EnterTree(){
		SetMultiplayerAuthority(int.Parse(Name));
		survivors[GetMultiplayerAuthority()] = this;
	}

	public static Survivor GetSurvivor(long authority){

		foreach (KeyValuePair<long, Survivor> kvp in survivors)
		{
			if (!IsInstanceValid(kvp.Value))
				survivors.Remove(kvp.Key);
		}

		if (!survivors.ContainsKey(authority)){
			return null;
		}

		return survivors[authority];

	}
	
	
	Camera3D camera;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		camera = GetNode<Camera3D>("neck/head/Camera");
		itemHolder = GetNode<ItemHolder>("%ItemHolder");
		itemHolder.SetMultiplayerAuthority(GetMultiplayerAuthority());
		itemPickupCast = GetNode<ItemPickupCast>("%ItemPickupCast");
		itemPickupCast.SetMultiplayerAuthority(GetMultiplayerAuthority());

		if (IsMultiplayerAuthority()){
			camera.MakeCurrent();
		}

		HandleMouseModeInputs();

	}

	public void HandleMouseModeInputs(){
		//toggle mouse being locked
		if (Input.IsActionJustPressed("escape")){
			freeMouse = !freeMouse;
		}
		else if (freeMouse && Input.IsActionJustPressed("leftClick")){
			freeMouse = false;
		}
		Input.MouseMode = freeMouse ? Input.MouseModeEnum.Visible : Input.MouseModeEnum.Captured; 

	}

	public override void _PhysicsProcess(double delta)
	{
		if (!IsMultiplayerAuthority())return;

		HandleMouseModeInputs();
		Move(delta);


	}


	public void Move(double delta){
		if (! IsOnFloor()){
			Velocity = Velocity + (Vector3.Down * (float)(GRAVITY * delta));
		}

		//Handle Jump
		if (Input.IsActionJustPressed("jump") && IsOnFloor()){
			Velocity = new Vector3(Velocity.X,JUMP_VELOCITY,Velocity.Z);
		}
		
		// Get the input direction and handle the movement/deceleration.
		Vector2 input_dir = Input.GetVector("left", "right", "forward", "backward");
		//we set the forward direction to where the body is facing.
		Vector3 direction = (Basis * new Vector3(input_dir.X, 0, input_dir.Y)).Normalized() * SPEED;
		if (direction != Vector3.Zero){
			// Y is up and down, so we don't want to change it.
			
			Velocity = new Vector3(
							(float)Mathf.Lerp(Velocity.X,direction.X,ACCEL * delta),
							Velocity.Y,
							(float)Mathf.Lerp(Velocity.Z,direction.Z,ACCEL * delta)
							);
		}
		else {
			Velocity = new Vector3(
							(float)Mathf.Lerp(Velocity.X,direction.X,DEACCEL * delta),
							Velocity.Y,
							(float)Mathf.Lerp(Velocity.Z,direction.Z,DEACCEL * delta)
							);
		}
		
		MoveAndSlide();
	}

	const double SENSITIVITY = 0.0015f;


	public override void _Input(InputEvent @event)
	{
		//don't move camera if mouse is captive or we're not the authority
		if (!IsMultiplayerAuthority() || freeMouse){return;}
		
		if (@event is InputEventMouseMotion){
			
			InputEventMouseMotion newEvent = @event as InputEventMouseMotion;

			Node3D head = GetNode<Node3D>("neck/head");

			head.RotateX((float)(-newEvent.Relative.Y * SENSITIVITY));


			RotateY((float)(-newEvent.Relative.X * SENSITIVITY));
			
			//Clamp head lookup and down
			head.Rotation = new Vector3(Mathf.Clamp(head.Rotation.X, Mathf.DegToRad(-90),Mathf.DegToRad(90)), head.Rotation.Y, head.Rotation.Z); 

		}
	}
		
	/// <summary>
	/// Called from floor items by the server.
	/// Adds the child to the inventory on the host instance which syncs with the client instance through the multiplayer spawner.
	/// </summary>
	/// <param name="item"></param>
	public ItemHolder GetItemHolder()
	{
		return itemHolder;
	}
		


}

/*
extends CharacterBody3D


var SPEED = 5.0
const ACCEL = 6.0
const DEACCEL = 8.0
const JUMP_VELOCITY = 4.5

var freeMouse = true

# Get the gravity from the project settings to be synced with RigidBody nodes.
var gravity = ProjectSettings.get_setting("physics/3d/default_gravity")

func _enter_tree():
	
	set_multiplayer_authority(name.to_int())
	

func _ready():
	if is_multiplayer_authority():
		%Camera.current = true
	
	Input.mouse_mode = Input.MOUSE_MODE_CAPTURED if !freeMouse else Input.MOUSE_MODE_VISIBLE;
	
	

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
		
		
		head.rotation.x = clamp(head.rotation.x, deg_to_rad(-90),deg_to_rad(90))*/
