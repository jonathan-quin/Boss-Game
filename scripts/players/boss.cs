using Godot;
using System;
using System.Collections.Generic;

public partial class boss : CharacterBody3D, TakeDamageInterface 
{

	public static Dictionary<long, boss> bosses = new Dictionary<long,boss>();

	const float SPEED = 12.0f;
	const float ACCEL = 1.4f;
	const float DEACCEL = 1.0f;
	const float JUMP_VELOCITY = 3f;

	const float GRAVITY = 10f;


	public PlayerUI playerUI;

	public float loadingTime = 1.0f;
	public Vector3 loadingPos = Vector3.Zero;

	public override void _EnterTree(){
		SetMultiplayerAuthority(int.Parse(Name));
		bosses[GetMultiplayerAuthority()] = this;
	}

	public static boss GetBoss(long authority)
	{

		foreach (KeyValuePair<long, boss> kvp in bosses)
		{
			if (!IsInstanceValid(kvp.Value))
				bosses.Remove(kvp.Key);
		}

		if (!bosses.ContainsKey(authority))
		{
			return null;
		}

		return bosses[authority];

	}

	public static bool AllBossesAreDead()
	{
		foreach (KeyValuePair<long, boss> kvp in bosses)
		{
			if (!IsInstanceValid(kvp.Value))
				bosses.Remove(kvp.Key);
		}

		return (bosses.Count == 0);

	}


	Camera3D camera;
	BossMonsterModel bossMesh;
	Node3D neck;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		camera = GetNode<Camera3D>("neck/head/Camera");
		bossMesh = GetNode<BossMonsterModel>("%bossMonsterRigged");
		neck = GetNode<Node3D>("neck");

		playerUI = (PlayerUI)GetTree().GetFirstNodeInGroup("playerUi");

		if (IsMultiplayerAuthority()){
			camera.MakeCurrent();
			GetNode<Node3D>("%headMesh").Visible = false;
		}
		else
		{
			//playerUI.Visible = false;
		}

	}


	public override void _PhysicsProcess(double delta)
	{
		if (!IsMultiplayerAuthority() || loadingTime >= 0 ){
			loadingTime -= (float) delta;
			GlobalPosition = loadingPos;
			return;
		}

		Move(delta);





		handleAttackInputs();
		
		Globals.health = (float) health;


	}

	double damageDealt = 50;

	public void handleAttackInputs()
	{
		if (!bossMesh.animationPlayer.IsPlaying())
		{
			bossMesh.animationPlayer.Play("idle");
		}

		if (Input.IsActionJustPressed("leftClick") && !Globals.freeMouse && !(bossMesh.animationPlayer.CurrentAnimation == "bite"))
		{
			bossMesh.animationPlayer.Play("bite");

			RpcId(Constants.SERVER_HOST_ID,"CreateDamageArea");
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void setLoadingPos(Vector3 loadingPos)
	{
		this.loadingPos = loadingPos;
	}
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void CreateDamageArea()
	{
		damageArea damageArea = GD.Load<PackedScene>(Constants.paths.damageAreaPath).Instantiate() as damageArea;

		damageArea.Position = bossMesh.GlobalPosition;
		damageArea.Rotation = bossMesh.Rotation;
		damageArea.RotateY(Mathf.DegToRad(-90));
		damageArea.Position += bossMesh.GlobalTransform.Basis.X * 1.7f;

		damageArea.damage = damageDealt;
		damageArea.targetEntity = TakeDamageInterface.TypeOfEntity.SURVIVOR.GetHashCode();

		//damage areas only need to exist on the server
		Globals.objectHolder.AddChild(damageArea);
		//Globals.multiplayerSpawner.Spawn(CustomMultiplayerSpawner.createSpawnRequest(damageArea,Constants.paths.damageAreaPath,"Transform","damage","targetEntity"));



	}


	Vector3 lastTargetDirection = Vector3.Zero;
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

		if (Globals.freeMouse)
		{
			input_dir = Vector2.Zero;
		}

		//we set the forward direction to where the body is facing.
		Vector3 direction = (neck.GlobalTransform.Basis * new Vector3(input_dir.X, 0, input_dir.Y)).Normalized() * SPEED;
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

		//if (direction != Vector3.Zero)
		//{
		//	lastTargetDirection = direction;
		//}

		//rotating the mesh
		float targetRotation = (float)(camera.GlobalRotation.Y + Mathf.DegToRad(90.0));
		float newRotation = (float)Mathf.LerpAngle(bossMesh.Rotation.Y, targetRotation, 3 * delta);

		bossMesh.Rotation = new Vector3(0, newRotation, 0);
		GetNode<Node3D>("%HurtDetect").Rotation = bossMesh.Rotation;
		GetNode<Node3D>("%HurtDetect").RotateY(Mathf.DegToRad(90));
		
		

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


			neck.RotateY((float)(-newEvent.Relative.X * SENSITIVITY));
			
			//Clamp head lookup and down
			head.Rotation = new Vector3(Mathf.Clamp(head.Rotation.X, Mathf.DegToRad(-90),Mathf.DegToRad(90)), head.Rotation.Y, head.Rotation.Z); 

		}
	}


	double _health = 100;
	bool _dead = false;
	public double health { get => _health; set => _health = value; }
	public bool dead { get => _dead; set => _dead = value; }
	public int _typeOfEntity = TakeDamageInterface.TypeOfEntity.BOSS.GetHashCode();
	public int typeOfEntity { get => _typeOfEntity; set => _typeOfEntity = value; }

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void TakeDamage(double amount)
	{
		if (Multiplayer.IsServer() && !IsMultiplayerAuthority()) {
			//GD.Print("Redirecting to server");
			RpcId(GetMultiplayerAuthority(),"TakeDamage",amount);
			
			return;
		}

		GetNode<SyncParticles>("%hurtParticles").EmittRPC();

		health -= amount;
		//GD.Print("taking damage");

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
			//GD.Print("redirecting die to server");
			RpcId(Constants.SERVER_HOST_ID,"Die");
		}else{

			//GD.Print("making spectator on server");

			Spectator spectator = GD.Load<PackedScene>(Constants.paths.spectatorPath).Instantiate() as Spectator;

			spectator.Transform = neck.GlobalTransform;

			spectator.targetAuthority = GetMultiplayerAuthority();

			Globals.multiplayerSpawner.Spawn(CustomMultiplayerSpawner.createSpawnRequest(spectator,Constants.paths.spectatorPath,"targetAuthority", "Transform"));
			
			camera.Current = false;

			QueueFree();
		}

	}
		


}




