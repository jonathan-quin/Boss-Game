using Godot;
using System;

public partial class baseItem : RigidBody3D
{
	
	[Export]
	public String pathToSelf;

	[Export]
	public double damageDealt = 5;

	public bool shouldShimmer = false;
	public bool heldByPlayer = false;

	public int targetMultiplayerAuthority = (int)Constants.SERVER_HOST_ID;

	/// <summary>
	/// Sets the multiplayer authority to the target authority.
	/// </summary>
	public override void _EnterTree()
	{

		//Constants.loadDataFromName(this, Name);

        
        SetMultiplayerAuthority((int)targetMultiplayerAuthority);
        //GD.Print("enter tree authority is ", GetMultiplayerAuthority(), " HBP ",heldByPlayer," claimed ",claimed, " I am the server: ",Multiplayer.IsServer());

        if (!heldByPlayer)
		{
			

			if (IsMultiplayerAuthority())
			{
				Freeze = false;
			}

			//GD.Print("created");
		} else if (IsMultiplayerAuthority()){

			//GD.Print("heyo");
			//GD.Print("my authority is ", GetMultiplayerAuthority());
			Survivor.GetSurvivor(GetMultiplayerAuthority()).GetItemHolder().TakeItem(this);
		}
		
		
		
	}


	
	/// <summary>
	/// Sets the shine animation and makes sure that the player who owns the item still exists. If they don't, it throws the item.
	/// </summary>
	/// <param name="delta"></param>
	public override void _Process(double delta)
	{

		if (!heldByPlayer) {
			AnimationPlayer animPlayer = GetNode<AnimationPlayer>("%LocalAnimationPlayer");
			if (shouldShimmer)
			{
				
				animPlayer.Play("shine");
			} else
			{
				animPlayer.Play("RESET");
			}
		} else {
			GetNode<AnimationPlayer>("%LocalAnimationPlayer").Play("RESET");

			//in case a player logs off
			if (IsMultiplayerAuthority() && ! IsInstanceValid(ItemHolder.localItemHolder) )
			{
				throwSelf(Transform);

            }

		}

        

		//GD.Print(Time.GetTicksMsec());

	}


	//Step one is asking the server if the item is available
	//PickedUp only ever gets changed on the server.
	bool claimed = false;
	public void giveToSurvivor(){
		//GD.Print("gts");
		RpcId(Constants.SERVER_HOST_ID, "_giveToSurvivorStep1", Multiplayer.GetUniqueId());
	}

	//called by client; runs on server
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void _giveToSurvivorStep1(long survivorID){
		if (claimed) return;

		//if player has all it's items, return

		claimed = true;

		baseItem newItem = GD.Load<PackedScene>(pathToSelf).Instantiate() as baseItem;

		//GD.Print("giving to survivor ",newItem);


		newItem.heldByPlayer = true;
		//newItem.SetMultiplayerAuthority((int)survivorID);


		newItem.targetMultiplayerAuthority = (int)survivorID;
		//GD.Print("the survivor id should be: ",survivorID);

		//newItem.Name = Constants.createName(newItem, "targetMultiplayerAuthority", "heldByPlayer");

		newItem.Position = Position;
		newItem.Rotation = Rotation;

        //Globals.objectHolder.AddChild(newItem);
		Globals.multiplayerSpawner.Spawn(CustomMultiplayerSpawner.createSpawnRequest(newItem,pathToSelf,"targetMultiplayerAuthority", "heldByPlayer","Position","Rotation"));


		QueueFree();

        
	}



	

        /// <summary>
        /// tells the server to throw and delete the item
        /// </summary>
        /// <param name="startTransform">The global transform from which to start</param>
        public void throwSelf(Transform3D startTransform){
			RpcId(Constants.SERVER_HOST_ID, "_throwSelf", startTransform);
		}

	//called by client; runs on server. Pass in global transform
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void _throwSelf(Transform3D startTransform){
		//GD.Print("we are starting the throw code");
		//GD.Print("instance child");
		baseItem newItem = GD.Load<PackedScene>(pathToSelf).Instantiate() as baseItem;

		//GD.Print(newItem);

		

		newItem.heldByPlayer = false;
		newItem.claimed = false;
		newItem.SetMultiplayerAuthority((int)Constants.SERVER_HOST_ID);


		//GD.Print("add child");
		Globals.objectHolder.AddChild(newItem,true);

		
		newItem.GlobalTransform = startTransform;
		newItem.GlobalPosition += startTransform.Basis.Z * -0.5f;

		float throwForce = 5f;
		newItem.ApplyImpulse(GlobalTransform.Basis.Z * -throwForce);

		QueueFree();
	}

    public virtual void Use()
    {
        GetNode<AnimationPlayer>("%SyncedAnimationPlayer").Play("swing");

		
		RpcId(Constants.SERVER_HOST_ID,"createDamageArea", (ItemHolder.localItemHolder.GetParent() as Node3D).GlobalTransform);
    }


	//only run on server, called anywhere. Damage areas only exist on the server.
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void createDamageArea(Transform3D transform){
		damageArea damageArea = GD.Load<PackedScene>(Constants.paths.damageAreaPath).Instantiate() as damageArea;

		damageArea.Transform = transform;
		damageArea.Position += transform.Basis.Z * -2.5f;

		damageArea.damage = damageDealt;
		damageArea.targetEntity = TakeDamageInterface.TypeOfEntity.BOSS.GetHashCode();
		
		//damage areas only need to exist on the server
        Globals.objectHolder.AddChild(damageArea);
		//Globals.multiplayerSpawner.Spawn(CustomMultiplayerSpawner.createSpawnRequest(damageArea,Constants.paths.damageAreaPath,"Transform","damage","targetEntity"));

	}


}
