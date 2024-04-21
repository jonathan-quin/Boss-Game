using Godot;
using System;
using System.Collections.Generic;

public partial class Survivor : CharacterBody3D , TakeDamageInterface
{

	public static Dictionary<long,Survivor> survivors = new Dictionary<long,Survivor>();

	const float SPEED = 5.0f;
	const float ACCEL = 6.0f;
	const float DEACCEL = 8.0f;
	const float JUMP_VELOCITY = 4.5f;

	const float GRAVITY = 10f;

	//prevents the player from moving until the game has synced with other clients. Every frame is decreased by delta.
	public float loadingTime = 1.0f;

	public ItemHolder itemHolder;
	public Label3D nameTag;
	ItemPickupCast itemPickupCast;
	public PlayerUI playerUI;

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

	public static bool AllSurvivorsAreDead()
	{
        foreach (KeyValuePair<long, Survivor> kvp in survivors)
        {
            if (!IsInstanceValid(kvp.Value))
                survivors.Remove(kvp.Key);
        }
		
		return (survivors.Count == 0);

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

		nameTag = GetNode<Label3D>("%nameTag");
		nameTag.Text = Globals.nameTagText;
		
		playerUI = GetNode<PlayerUI>("%PlayerUI");
		playerUI.SetMultiplayerAuthority(GetMultiplayerAuthority());


        if (IsMultiplayerAuthority()){
			camera.MakeCurrent();
			GetNode<Node3D>("%survivor Monster rigged for game").Visible = false;
			 
		} else
		{
			camera.ClearCurrent();
			playerUI.Visible = false;
		}

		lobbyInterface.instance.resume();


	}

	

	public override void _PhysicsProcess(double delta)
	{
		if (!IsMultiplayerAuthority() || loadingTime >= 0){
			loadingTime -= (float) delta;
			return;
		}

		Move(delta);


	}


	public void Move(double delta){
		//if (! IsOnFloor()){
			Velocity = Velocity + (Vector3.Down * (float)(GRAVITY * delta));
		//}

		//Handle Jump
		if (Input.IsActionJustPressed("jump") && IsOnFloor()){
			Velocity = new Vector3(Velocity.X,JUMP_VELOCITY,Velocity.Z);
		}
		
		// Get the input direction and handle the movement/deceleration.
		Vector2 input_dir = Input.GetVector("left", "right", "forward", "backward");
		//we set the forward direction to where the body is facing.
		Vector3 direction = (Basis * new Vector3(input_dir.X, 0, input_dir.Y)).Normalized() * SPEED;

		if (Globals.freeMouse) direction = Vector3.Zero;

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
		
		if (false && IsOnFloor() && Velocity.DistanceTo(Vector3.Zero) > 0.2 && !GetNode<AudioStreamPlayer3D>("%walkingSound").Playing)
		{
			GetNode<SoundSyncer>("%walkingSound").PlayRPC();
			//GD.Print("Debug, ", Multiplayer.GetUniqueId());
		}
		
		MoveAndSlide();
	}

	const double SENSITIVITY = 0.0015f;

    public override void _Input(InputEvent @event)
	{
		//don't move camera if mouse is captive or we're not the authority
		if (!IsMultiplayerAuthority() || Globals.freeMouse){return;}
		
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

	
	double _health = 100;
	bool _dead = false;
    public double health { get => _health; set => _health = value; }
	public bool dead { get => _dead; set => _dead = value; }

	
	public int _typeOfEntity = TakeDamageInterface.TypeOfEntity.SURVIVOR.GetHashCode();
	public int typeOfEntity { get => _typeOfEntity; set => _typeOfEntity = value; }

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void TakeDamage(double amount)
    {
		if (Multiplayer.IsServer() && !IsMultiplayerAuthority()) {
			RpcId(GetMultiplayerAuthority(),"TakeDamage",amount);
			return;
		}

        GetNode<SyncParticles>("%hurtParticles").EmittRPC();

        health -= amount;

		if (health <= 0 && !dead){
			//GD.Print("dying!");
			dead = true;
			Die();
		}

    }

	/// <summary>
	/// Tells the server's instance of the client to queue free
	/// </summary>
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void Die(){
		
		if (!Multiplayer.IsServer()) {
			RpcId(Constants.SERVER_HOST_ID,"Die");
		}else{

			Spectator spectator = GD.Load<PackedScene>(Constants.paths.spectatorPath).Instantiate() as Spectator;

			Node3D head = GetNode<Node3D>("neck/head");

			spectator.Transform = head.GlobalTransform;

			spectator.targetAuthority = GetMultiplayerAuthority();

			Globals.multiplayerSpawner.Spawn(CustomMultiplayerSpawner.createSpawnRequest(spectator,Constants.paths.spectatorPath,"targetAuthority", "Transform"));
			


			QueueFree();
		}

	}



}




