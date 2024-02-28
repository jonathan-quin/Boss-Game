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
		}
		
		
		
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
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

		newItem.SetMultiplayerAuthority((int)Constants.SERVER_HOST_ID);

		Globals.objectHolder.AddChild(newItem);

		newItem.GlobalPosition = GlobalPosition;
		newItem.GlobalRotation = GlobalRotation;

		newItem.heldByPlayer = true;
		newItem.claimed = true;
		newItem.SetMultiplayerAuthority((int)survivorID);

		QueueFree();

        
	}

    public virtual void Use()
    {
        GetNode<AnimationPlayer>("%LocalAnimationPlayer").Play("swing");
    }
}
