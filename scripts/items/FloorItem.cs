using Godot;
using System;

public partial class FloorItem : RigidBody3D
{

	[Export]
	public String heldFormPath;

	public bool shouldShimmer = false;

	public override void _EnterTree()
	{

		
		SetMultiplayerAuthority((int)Constants.SERVER_HOST_ID);

		if (IsMultiplayerAuthority())
		{
			Freeze = false;
		}
		
		
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        AnimationPlayer animPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
        if (shouldShimmer)
		{
            
			animPlayer.Play("shine");
        } else
		{
            animPlayer.Play("RESET");
        }
	}


	bool pickedUp = false;
	public void giveToSurvivor(long survivorID){
		
		RpcId(Constants.SERVER_HOST_ID,"_giveToSurvivor", survivorID);
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void _giveToSurvivor(long survivorID){


		if (pickedUp) return;
		pickedUp = true;

		Survivor.GetSurvivor(survivorID).TakeItem(heldFormPath);
		QueueFree();
	}


}
