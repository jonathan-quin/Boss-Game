using Godot;
using System;

public partial class SoundSyncer : AudioStreamPlayer3D
{
	
	[Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
	public void PlayRPC(float offset = 0.0f,int origin = -1)
	{
		if (origin == Multiplayer.GetUniqueId()) return;
		base.Play(offset);
		//GD.Print("Playing");
		if (origin == -1)
		Rpc( "PlayRPC", offset,Multiplayer.GetUniqueId());
	}
}
