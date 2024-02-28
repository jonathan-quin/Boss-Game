using Godot;
using System;

public partial class baseItem : RigidBody3D
{
	
	[Export]
	public String pathToSelf;

	public bool shouldShimmer = false;
	public bool heldByPlayer = false;

	public override void _EnterTree()
	{

		if (!heldByPlayer)
		{
			SetMultiplayerAuthority((int)Constants.SERVER_HOST_ID);

			if (IsMultiplayerAuthority())
			{
				Freeze = false;
			}

			//GD.Print("created");
		} else if (IsMultiplayerAuthority()){

			//GD.Print("heyo");

			Survivor.GetSurvivor(GetMultiplayerAuthority()).GetItemHolder().TakeItem(this);
		}
		
		
		
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("yes we print");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
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

		}

        

		//GD.Print(Time.GetTicksMsec());

	}


	//Step one is asking the server if the item is available
	//PickedUp only ever gets changed on the server.
	bool claimed = false;
	public void giveToSurvivor(){
		//GD.Print("gts");
		RpcId(Constants.SERVER_HOST_ID, "_giveToSurvivorStep1", GetMultiplayerAuthority());
	}

	//called by client; runs on server
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void _giveToSurvivorStep1(long survivorID){
		if (claimed) return;

		//if player has all it's items, return

		claimed = true;

		baseItem newItem = GD.Load<PackedScene>(pathToSelf).Instantiate() as baseItem;

		GD.Print(newItem);

		newItem.SetMultiplayerAuthority((int)Constants.SERVER_HOST_ID);

		newItem.heldByPlayer = true;
		newItem.claimed = true;
		newItem.SetMultiplayerAuthority((int)survivorID);

		Globals.objectHolder.AddChild(newItem,true);

		newItem.GlobalPosition = GlobalPosition;
		newItem.GlobalRotation = GlobalRotation;

		

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

		//GD.Print("instance child");
		baseItem newItem = GD.Load<PackedScene>(pathToSelf).Instantiate() as baseItem;

		//GD.Print(newItem);

		newItem.SetMultiplayerAuthority((int)Constants.SERVER_HOST_ID);

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
    }
}
