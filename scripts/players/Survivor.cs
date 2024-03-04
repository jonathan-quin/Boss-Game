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
			GetNode<Node3D>("%headMesh").Visible = false;
		} else
		{
			camera.ClearCurrent();
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



