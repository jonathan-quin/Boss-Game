using Godot;
using System;

public partial class FloorItem : Node
{

	[Export]
	public PackedScene heldForm;



	private dynamic Globals;
	public override void _EnterTree()
	{
		Globals = GetNode<Node>("/root/Globals");

		
		SetMultiplayerAuthority(Globals.serverHostID);
		
		
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}


	bool pickedUp = false;
	public void giveToSurvivor(Survivor survivor){
		
		RpcId(Constants.SERVER_HOST_ID,"_giveToSurvivor",survivor.GetMultiplayerAuthority());
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer, CallLocal = true, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void _giveToSurvivor(long survivorID){

		if (pickedUp) return;
		pickedUp = true;

		Survivor.GetSurvivor(survivorID).TakeItem(heldForm);
		QueueFree();
	}


}
