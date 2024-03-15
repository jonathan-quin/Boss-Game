using Godot;
using System;

public partial class SyncParticles : CpuParticles3D
{
    [Rpc(MultiplayerApi.RpcMode.AnyPeer, TransferMode = MultiplayerPeer.TransferModeEnum.Reliable)]
    public void EmittRPC(int origin = -1)
    {
        if (origin == Multiplayer.GetUniqueId()) return;
        Emitting = true;
        if (origin == -1)
            Rpc("EmittRPC", Multiplayer.GetUniqueId());
    }
}
