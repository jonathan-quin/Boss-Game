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

		GD.Print(Time.GetTicksMsec());

	}


	//Step one is asking the server if the item is available
	//PickedUp only ever gets changed on the server.
	bool pickedUp = false;
	public void giveToSurvivor(){
		//GD.Print("gts");
		RpcId(Constants.SERVER_HOST_ID, "_giveToSurvivorStep1", GetMultiplayerAuthority());
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void _giveToSurvivorStep1(long survivorID){
		//GD.Print("1");
		if (pickedUp) return;
		pickedUp = true;
        //GD.Print("2");
        RpcId(survivorID, "_giveToSurvivorStep2", survivorID);
	}
	//step 2 is called by the server and run by the client again
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void _giveToSurvivorStep2(long survivorID)
    {
		bool playerTookItem = Survivor.GetSurvivor(survivorID).GetItemHolder().TakeItem(heldFormPath);
        GD.Print("3");
        RpcId(Constants.SERVER_HOST_ID, "_finalize", playerTookItem);
    }
	//run on the server, called by the client
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void _finalize(bool pickedUp)
    {
        //GD.Print("4");
        if (pickedUp)
		{
			QueueFree();
		}
		else
		{
			pickedUp = false;
		}
    }


}
